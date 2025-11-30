using System.Collections.Generic;

namespace LibraryManager.Models
{
    /// <summary>
    /// Модель читателя библиотеки
    /// </summary>
    public class Reader
    {
        /// <summary>
        /// Уникальный идентификатор читателя
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Имя читателя
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email читателя
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Список ID книг, которые читатель взял
        /// </summary>
        public List<int> BorrowedBookIds { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Reader()
        {
            BorrowedBookIds = new List<int>();
        }

        /// <summary>
        /// Переопределение метода ToString() для удобного вывода
        /// </summary>
        public override string ToString()
        {
            return $"ID: {Id} | {Name} | {Email} | Книг на руках: {BorrowedBookIds.Count}";
        }
    }
}

