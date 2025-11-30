using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using LibraryManager.Models;

namespace LibraryManager.Services
{
    /// <summary>
    /// Сервис для управления библиотекой
    /// Отвечает за все операции с книгами, читателями и выдачами
    /// </summary>
    public class LibraryService
    {
        private List<Book> _books;
        private List<Reader> _readers;
        private List<Borrowing> _borrowings;
        private string _filePath;
        private int _nextBookId;
        private int _nextReaderId;
        private int _nextBorrowingId;

        // События для уведомлений (Publisher-Subscriber паттерн)
        public event Action<string>? BookBorrowed;
        public event Action<string>? BookReturned;

        /// <summary>
        /// Конструктор библиотеки
        /// </summary>
        /// <param name="filePath">Путь к файлу для сохранения данных</param>
        public LibraryService(string filePath)
        {
            this._filePath = filePath;
            this._books = new List<Book>();
            this._readers = new List<Reader>();
            this._borrowings = new List<Borrowing>();
            this._nextBookId = 1;
            this._nextReaderId = 1;
            this._nextBorrowingId = 1;
            LoadData();
        }

        /// <summary>
        /// Загрузка данных из JSON файла
        /// </summary>
        public void LoadData()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    var data = JsonSerializer.Deserialize<LibraryData>(json);

                    if (data != null)
                    {
                        _books = data.Books ?? new List<Book>();
                        _readers = data.Readers ?? new List<Reader>();
                        _borrowings = data.Borrowings ?? new List<Borrowing>();

                        // Обновляем счетчики ID
                        if (_books.Count > 0)
                            _nextBookId = _books.Max(b => b.Id) + 1;
                        if (_readers.Count > 0)
                            _nextReaderId = _readers.Max(r => r.Id) + 1;
                        if (_borrowings.Count > 0)
                            _nextBorrowingId = _borrowings.Max(br => br.Id) + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        /// <summary>
        /// Сохранение данных в JSON файл
        /// </summary>
        public void SaveData()
        {
            try
            {
                // Создаем директорию, если её нет
                string? directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var data = new LibraryData
                {
                    Books = _books,
                    Readers = _readers,
                    Borrowings = _borrowings
                };

                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при сохранении данных: {ex.Message}");
            }
        }

        /// <summary>
        /// Добавление новой книги в библиотеку
        /// </summary>
        public void AddBook(string title, string author, string genre, int year)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Ошибка: Название книги не может быть пустым!");
                return;
            }

            Book book = new Book
            {
                Id = _nextBookId++,
                Title = title.Trim(),
                Author = author?.Trim() ?? string.Empty,
                Genre = genre?.Trim() ?? string.Empty,
                Year = year,
                IsAvailable = true
            };

            _books.Add(book);
            SaveData();
            Console.WriteLine($"✓ Книга '{title}' добавлена (ID: {book.Id})");
        }

        /// <summary>
        /// Удаление книги из библиотеки
        /// </summary>
        public void RemoveBook(int bookId)
        {
            Book? book = _books.FirstOrDefault(b => b.Id == bookId);
            if (book != null)
            {
                if (!book.IsAvailable)
                {
                    Console.WriteLine($"✗ Нельзя удалить книгу '{book.Title}' - она выдана читателю!");
                    return;
                }

                _books.Remove(book);
                SaveData();
                Console.WriteLine($"✓ Книга '{book.Title}' удалена");
            }
            else
            {
                Console.WriteLine($"✗ Книга с ID {bookId} не найдена");
            }
        }

        /// <summary>
        /// Поиск книг по названию, автору или жанру
        /// </summary>
        public List<Book> SearchBooks(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<Book>();
            }

            return _books.Where(b =>
                b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                b.Genre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        /// <summary>
        /// Фильтрация книг по жанру и/или доступности
        /// </summary>
        public List<Book> FilterBooks(string? genre = null, bool? isAvailable = null)
        {
            var query = _books.AsQueryable();

            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(b => b.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
            }

            if (isAvailable.HasValue)
            {
                query = query.Where(b => b.IsAvailable == isAvailable.Value);
            }

            return query.ToList();
        }

        /// <summary>
        /// Сортировка книг по различным критериям
        /// </summary>
        public List<Book> SortBooks(string sortBy)
        {
            switch (sortBy.ToLower())
            {
                case "title":
                    return _books.OrderBy(b => b.Title).ToList();
                case "author":
                    return _books.OrderBy(b => b.Author).ToList();
                case "year":
                    return _books.OrderByDescending(b => b.Year).ToList();
                default:
                    return _books;
            }
        }

        /// <summary>
        /// Добавление нового читателя
        /// </summary>
        public void AddReader(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Ошибка: Имя читателя не может быть пустым!");
                return;
            }

            Reader reader = new Reader
            {
                Id = _nextReaderId++,
                Name = name.Trim(),
                Email = email?.Trim() ?? string.Empty
            };

            _readers.Add(reader);
            SaveData();
            Console.WriteLine($"✓ Читатель '{name}' добавлен (ID: {reader.Id})");
        }

        /// <summary>
        /// Выдача книги читателю
        /// </summary>
        public void BorrowBook(int bookId, int readerId)
        {
            Book? book = _books.FirstOrDefault(b => b.Id == bookId);
            Reader? reader = _readers.FirstOrDefault(r => r.Id == readerId);

            if (book == null)
            {
                Console.WriteLine($"✗ Книга с ID {bookId} не найдена");
                return;
            }

            if (reader == null)
            {
                Console.WriteLine($"✗ Читатель с ID {readerId} не найден");
                return;
            }

            if (!book.IsAvailable)
            {
                Console.WriteLine($"✗ Книга '{book.Title}' уже выдана");
                return;
            }

            Borrowing borrowing = new Borrowing
            {
                Id = _nextBorrowingId++,
                BookId = bookId,
                ReaderId = readerId,
                BorrowDate = DateTime.Now
            };

            book.IsAvailable = false;
            book.BorrowHistory.Add(borrowing);
            reader.BorrowedBookIds.Add(bookId);
            _borrowings.Add(borrowing);
            SaveData();

            string message = $"✓ Книга '{book.Title}' выдана читателю '{reader.Name}'";
            Console.WriteLine(message);
            BookBorrowed?.Invoke(message);
        }

        /// <summary>
        /// Возврат книги в библиотеку
        /// </summary>
        public void ReturnBook(int bookId)
        {
            Book? book = _books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                Console.WriteLine($"✗ Книга с ID {bookId} не найдена");
                return;
            }

            Borrowing? borrowing = _borrowings
                .FirstOrDefault(br => br.BookId == bookId && !br.IsReturned);

            if (borrowing == null)
            {
                Console.WriteLine($"✗ Книга '{book.Title}' не была выдана");
                return;
            }

            borrowing.ReturnDate = DateTime.Now;
            book.IsAvailable = true;

            Reader? reader = _readers.FirstOrDefault(r => r.Id == borrowing.ReaderId);
            if (reader != null)
            {
                reader.BorrowedBookIds.Remove(bookId);
            }

            SaveData();

            string message = $"✓ Книга '{book.Title}' возвращена в библиотеку";
            Console.WriteLine(message);
            BookReturned?.Invoke(message);
        }

        /// <summary>
        /// Получение истории выдач с фильтрацией
        /// </summary>
        public List<Borrowing> GetBorrowHistory(int? bookId = null, int? readerId = null)
        {
            var query = _borrowings.AsQueryable();

            if (bookId.HasValue)
            {
                query = query.Where(br => br.BookId == bookId.Value);
            }

            if (readerId.HasValue)
            {
                query = query.Where(br => br.ReaderId == readerId.Value);
            }

            return query.OrderByDescending(br => br.BorrowDate).ToList();
        }

        /// <summary>
        /// Показать статистику библиотеки
        /// </summary>
        public void ShowStatistics()
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║     СТАТИСТИКА БИБЛИОТЕКИ        ║");
            Console.WriteLine("╚══════════════════════════════════╝");
            Console.WriteLine($"Всего книг: {_books.Count}");
            Console.WriteLine($"Доступно книг: {_books.Count(b => b.IsAvailable)}");
            Console.WriteLine($"Выдано книг: {_books.Count(b => !b.IsAvailable)}");
            Console.WriteLine($"Всего читателей: {_readers.Count}");
            Console.WriteLine($"Всего выдач: {_borrowings.Count}");
            Console.WriteLine($"Активных выдач: {_borrowings.Count(br => !br.IsReturned)}");

            // Топ-5 жанров (используем LINQ GroupBy)
            if (_books.Count > 0)
            {
                var topGenres = _books
                    .GroupBy(b => b.Genre)
                    .Select(g => new { Genre = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5);

                Console.WriteLine("\n📚 Топ-5 жанров:");
                foreach (var genre in topGenres)
                {
                    Console.WriteLine($"  - {genre.Genre}: {genre.Count} книг");
                }

                // Топ-5 авторов
                var topAuthors = _books
                    .GroupBy(b => b.Author)
                    .Select(g => new { Author = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5);

                Console.WriteLine("\n✍️ Топ-5 авторов:");
                foreach (var author in topAuthors)
                {
                    Console.WriteLine($"  - {author.Author}: {author.Count} книг");
                }
            }
        }

        // Геттеры для доступа к данным
        public List<Book> GetAllBooks() => _books;
        public List<Reader> GetAllReaders() => _readers;
        public List<Borrowing> GetAllBorrowings() => _borrowings;
    }
}

