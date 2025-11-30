using System;
using System.IO;
using System.Linq;
using LibraryManager.Models;
using LibraryManager.Services;

namespace LibraryManager
{
    /// <summary>
    /// Главный класс программы
    /// Отвечает за пользовательский интерфейс и взаимодействие с пользователем
    /// </summary>
    class Program
    {
        // Путь к файлу данных (относительно папки программы)
        private static readonly string DataFilePath = GetDataFilePath();

        /// <summary>
        /// Определяет полный путь к файлу данных и создает директорию, если ее нет.
        /// </summary>
        /// <returns>Полный путь к файлу данных.</returns>
        private static string GetDataFilePath()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dataDirectory = Path.Combine(baseDirectory, "Data");

            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
            return Path.Combine(dataDirectory, "library.json");
        }

        /// <summary>
        /// Точка входа в программу
        /// </summary>
        static void Main(string[] args)
        {
            // Создаем экземпляр сервиса для управления библиотекой
            LibraryService library = new LibraryService(DataFilePath);

            // Подписка на события (Publisher-Subscriber паттерн)
            library.BookBorrowed += (message) =>
            {
                Console.WriteLine($"[СОБЫТИЕ] {message}");
            };

            library.BookReturned += (message) =>
            {
                Console.WriteLine($"[СОБЫТИЕ] {message}");
            };

            // Приветственное сообщение
            Console.WriteLine("╔══════════════════════════════════╗");
            Console.WriteLine("║   СИСТЕМА УПРАВЛЕНИЯ БИБЛИОТЕКОЙ ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            // Главный цикл программы
            while (true)
            {
                ShowMainMenu();

                // Читаем выбор пользователя
                string? choice = Console.ReadLine();

                // Обрабатываем выбор
                switch (choice)
                {
                    case "1":
                        ShowBookMenu(library);
                        break;
                    case "2":
                        ShowReaderMenu(library);
                        break;
                    case "3":
                        ShowBorrowMenu(library);
                        break;
                    case "4":
                        library.ShowStatistics();
                        break;
                    case "0":
                        Console.WriteLine("\nДо свидания! Данные сохранены.");
                        return;
                    default:
                        Console.WriteLine("✗ Неверный выбор! Попробуйте снова.");
                        break;
                }

                // Пауза перед следующим выводом меню
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        /// <summary>
        /// Отображает главное меню
        /// </summary>
        static void ShowMainMenu()
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║           ГЛАВНОЕ МЕНЮ          ║");
            Console.WriteLine("╚══════════════════════════════════╝");
            Console.WriteLine("1. Управление книгами");
            Console.WriteLine("2. Управление читателями");
            Console.WriteLine("3. Выдача/возврат книг");
            Console.WriteLine("4. Статистика");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие: ");
        }

        /// <summary>
        /// Меню управления книгами
        /// </summary>
        static void ShowBookMenu(LibraryService library)
        {
            while (true)
            {
                Console.WriteLine("\n╔══════════════════════════════════╗");
                Console.WriteLine("║        УПРАВЛЕНИЕ КНИГАМИ        ║");
                Console.WriteLine("╚══════════════════════════════════╝");
                Console.WriteLine("1. Добавить книгу");
                Console.WriteLine("2. Удалить книгу");
                Console.WriteLine("3. Показать все книги");
                Console.WriteLine("4. Поиск книг");
                Console.WriteLine("5. Фильтрация книг");
                Console.WriteLine("6. Сортировка книг");
                Console.WriteLine("0. Назад");
                Console.Write("\nВыберите действие: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HandleAddBook(library);
                        break;

                    case "2":
                        HandleRemoveBook(library);
                        break;

                    case "3":
                        HandleShowAllBooks(library);
                        break;

                    case "4":
                        HandleSearchBooks(library);
                        break;

                    case "5":
                        HandleFilterBooks(library);
                        break;

                    case "6":
                        HandleSortBooks(library);
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("✗ Неверный выбор!");
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        /// <summary>
        /// Обрабатывает добавление книги
        /// </summary>
        static void HandleAddBook(LibraryService library)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║        ДОБАВЛЕНИЕ КНИГИ          ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.Write("Название: ");
            string? title = Console.ReadLine();

            Console.Write("Автор: ");
            string? author = Console.ReadLine();

            Console.Write("Жанр: ");
            string? genre = Console.ReadLine();

            Console.Write("Год издания: ");
            if (int.TryParse(Console.ReadLine(), out int year))
            {
                library.AddBook(title ?? "", author ?? "", genre ?? "", year);
            }
            else
            {
                Console.WriteLine("✗ Неверный формат года");
            }
        }

        /// <summary>
        /// Обрабатывает удаление книги
        /// </summary>
        static void HandleRemoveBook(LibraryService library)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║        УДАЛЕНИЕ КНИГИ           ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.Write("ID книги для удаления: ");
            if (int.TryParse(Console.ReadLine(), out int bookId))
            {
                library.RemoveBook(bookId);
            }
            else
            {
                Console.WriteLine("✗ Неверный формат ID");
            }
        }

        /// <summary>
        /// Обрабатывает отображение всех книг
        /// </summary>
        static void HandleShowAllBooks(LibraryService library)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║           ВСЕ КНИГИ              ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            var allBooks = library.GetAllBooks();
            if (allBooks.Count == 0)
            {
                Console.WriteLine("Книг нет! Добавьте первую книгу.");
            }
            else
            {
                foreach (var book in allBooks)
                {
                    Console.WriteLine(book);
                }
            }
        }

        /// <summary>
        /// Обрабатывает поиск книг
        /// </summary>
        static void HandleSearchBooks(LibraryService library)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║          ПОИСК КНИГ               ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.Write("Поисковый запрос: ");
            string? search = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(search))
            {
                Console.WriteLine("✗ Поисковый запрос не может быть пустым");
                return;
            }

            var searchResults = library.SearchBooks(search);
            if (searchResults.Count == 0)
            {
                Console.WriteLine($"Не найдено книг по запросу '{search}'");
            }
            else
            {
                Console.WriteLine($"\nНайдено книг: {searchResults.Count}");
                foreach (var book in searchResults)
                {
                    Console.WriteLine(book);
                }
            }
        }

        /// <summary>
        /// Обрабатывает фильтрацию книг
        /// </summary>
        static void HandleFilterBooks(LibraryService library)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║       ФИЛЬТРАЦИЯ КНИГ            ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.Write("Жанр (Enter - все): ");
            string? filterGenre = Console.ReadLine();

            Console.Write("Доступность (true/false/Enter - все): ");
            string? availabilityInput = Console.ReadLine();
            bool? isAvailable = null;
            if (!string.IsNullOrWhiteSpace(availabilityInput) && bool.TryParse(availabilityInput, out bool avail))
            {
                isAvailable = avail;
            }

            var filteredBooks = library.FilterBooks(
                string.IsNullOrEmpty(filterGenre) ? null : filterGenre,
                isAvailable
            );

            if (filteredBooks.Count == 0)
            {
                Console.WriteLine("Задач не найдено");
            }
            else
            {
                Console.WriteLine($"\nНайдено книг: {filteredBooks.Count}");
                foreach (var book in filteredBooks)
                {
                    Console.WriteLine(book);
                }
            }
        }

        /// <summary>
        /// Обрабатывает сортировку книг
        /// </summary>
        static void HandleSortBooks(LibraryService library)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║        СОРТИРОВКА КНИГ           ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.WriteLine("Выберите критерий сортировки:");
            Console.WriteLine("1. По названию");
            Console.WriteLine("2. По автору");
            Console.WriteLine("3. По году издания");
            Console.Write("Ваш выбор: ");

            string? choice = Console.ReadLine();
            string sortBy = choice switch
            {
                "1" => "title",
                "2" => "author",
                "3" => "year",
                _ => "title"
            };

            var sortedBooks = library.SortBooks(sortBy);

            Console.WriteLine($"\nКниги отсортированы по: {sortBy}");
            foreach (var book in sortedBooks)
            {
                Console.WriteLine(book);
            }
        }

        /// <summary>
        /// Меню управления читателями
        /// </summary>
        static void ShowReaderMenu(LibraryService library)
        {
            while (true)
            {
                Console.WriteLine("\n╔══════════════════════════════════╗");
                Console.WriteLine("║      УПРАВЛЕНИЕ ЧИТАТЕЛЯМИ     ║");
                Console.WriteLine("╚══════════════════════════════════╝");
                Console.WriteLine("1. Добавить читателя");
                Console.WriteLine("2. Показать всех читателей");
                Console.WriteLine("0. Назад");
                Console.Write("\nВыберите действие: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HandleAddReader(library);
                        break;

                    case "2":
                        HandleShowAllReaders(library);
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("✗ Неверный выбор!");
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        /// <summary>
        /// Обрабатывает добавление читателя
        /// </summary>
        static void HandleAddReader(LibraryService library)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║      ДОБАВЛЕНИЕ ЧИТАТЕЛЯ         ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.Write("Имя: ");
            string? name = Console.ReadLine();

            Console.Write("Email: ");
            string? email = Console.ReadLine();

            library.AddReader(name ?? "", email ?? "");
        }

        /// <summary>
        /// Обрабатывает отображение всех читателей
        /// </summary>
        static void HandleShowAllReaders(LibraryService library)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║        ВСЕ ЧИТАТЕЛИ              ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            var readers = library.GetAllReaders();
            if (readers.Count == 0)
            {
                Console.WriteLine("Читателей нет! Добавьте первого читателя.");
            }
            else
            {
                foreach (var reader in readers)
                {
                    Console.WriteLine(reader);
                }
            }
        }

        /// <summary>
        /// Меню выдачи и возврата книг
        /// </summary>
        static void ShowBorrowMenu(LibraryService library)
        {
            while (true)
            {
                Console.WriteLine("\n╔══════════════════════════════════╗");
                Console.WriteLine("║      ВЫДАЧА/ВОЗВРАТ КНИГ         ║");
                Console.WriteLine("╚══════════════════════════════════╝");
                Console.WriteLine("1. Выдать книгу");
                Console.WriteLine("2. Вернуть книгу");
                Console.WriteLine("3. История выдач");
                Console.WriteLine("0. Назад");
                Console.Write("\nВыберите действие: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HandleBorrowBook(library);
                        break;

                    case "2":
                        HandleReturnBook(library);
                        break;

                    case "3":
                        HandleBorrowHistory(library);
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("✗ Неверный выбор!");
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        /// <summary>
        /// Обрабатывает выдачу книги
        /// </summary>
        static void HandleBorrowBook(LibraryService library)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║          ВЫДАЧА КНИГИ             ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.Write("ID книги: ");
            if (int.TryParse(Console.ReadLine(), out int bookId))
            {
                Console.Write("ID читателя: ");
                if (int.TryParse(Console.ReadLine(), out int readerId))
                {
                    library.BorrowBook(bookId, readerId);
                }
                else
                {
                    Console.WriteLine("✗ Неверный формат ID читателя");
                }
            }
            else
            {
                Console.WriteLine("✗ Неверный формат ID книги");
            }
        }

        /// <summary>
        /// Обрабатывает возврат книги
        /// </summary>
        static void HandleReturnBook(LibraryService library)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║         ВОЗВРАТ КНИГИ            ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.Write("ID книги для возврата: ");
            if (int.TryParse(Console.ReadLine(), out int returnBookId))
            {
                library.ReturnBook(returnBookId);
            }
            else
            {
                Console.WriteLine("✗ Неверный формат ID");
            }
        }

        /// <summary>
        /// Обрабатывает просмотр истории выдач
        /// </summary>
        static void HandleBorrowHistory(LibraryService library)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║        ИСТОРИЯ ВЫДАЧ             ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.Write("ID книги (Enter - все): ");
            string? bookIdInput = Console.ReadLine();

            Console.Write("ID читателя (Enter - все): ");
            string? readerIdInput = Console.ReadLine();

            int? bookIdFilter = null;
            int? readerIdFilter = null;

            if (!string.IsNullOrWhiteSpace(bookIdInput) && int.TryParse(bookIdInput, out int bid))
                bookIdFilter = bid;
            if (!string.IsNullOrWhiteSpace(readerIdInput) && int.TryParse(readerIdInput, out int rid))
                readerIdFilter = rid;

            var history = library.GetBorrowHistory(bookIdFilter, readerIdFilter);
            if (history.Count == 0)
            {
                Console.WriteLine("История выдач пуста");
            }
            else
            {
                Console.WriteLine($"\nНайдено записей: {history.Count}");
                foreach (var borrowing in history)
                {
                    Console.WriteLine(borrowing);
                }
            }
        }
    }
}

