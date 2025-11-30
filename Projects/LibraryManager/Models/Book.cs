using System;
using System.Collections.Generic;

namespace LibraryManager.Models
{
    /// <summary>
    /// Модель книги в библиотеке
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Уникальный идентификатор книги
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название книги
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Автор книги
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// Жанр книги
        /// </summary>
        public string Genre { get; set; } = string.Empty;

        /// <summary>
        /// Год издания
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Доступна ли книга для выдачи
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// История выдач книги
        /// </summary>
        public List<Borrowing> BorrowHistory { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Book()
        {
            BorrowHistory = new List<Borrowing>();
            IsAvailable = true;
        }

        /// <summary>
        /// Переопределение метода ToString() для удобного вывода
        /// </summary>
        public override string ToString()
        {
            return $"ID: {Id} | {Title} | {Author} | {Genre} | {Year} | " +
                   $"Доступна: {(IsAvailable ? "Да" : "Нет")}";
        }
    }
}

