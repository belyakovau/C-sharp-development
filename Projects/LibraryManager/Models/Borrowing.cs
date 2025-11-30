using System;

namespace LibraryManager.Models
{
    /// <summary>
    /// Модель выдачи книги читателю
    /// </summary>
    public class Borrowing
    {
        /// <summary>
        /// Уникальный идентификатор выдачи
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID книги
        /// </summary>
        public int BookId { get; set; }

        /// <summary>
        /// ID читателя
        /// </summary>
        public int ReaderId { get; set; }

        /// <summary>
        /// Дата выдачи книги
        /// </summary>
        public DateTime BorrowDate { get; set; }

        /// <summary>
        /// Дата возврата книги (null, если книга еще не возвращена)
        /// </summary>
        public DateTime? ReturnDate { get; set; }

        /// <summary>
        /// Возвращена ли книга
        /// </summary>
        public bool IsReturned => ReturnDate.HasValue;

        /// <summary>
        /// Переопределение метода ToString() для удобного вывода
        /// </summary>
        public override string ToString()
        {
            string status = IsReturned ? "Возвращена" : "На руках";
            string returnInfo = ReturnDate.HasValue
                ? $" | Возвращена: {ReturnDate.Value:dd.MM.yyyy}"
                : "";
            return $"ID: {Id} | Книга ID: {BookId} | Читатель ID: {ReaderId} | " +
                   $"Взята: {BorrowDate:dd.MM.yyyy} | {status}{returnInfo}";
        }
    }
}

