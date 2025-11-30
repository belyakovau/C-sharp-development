using System.Collections.Generic;

namespace LibraryManager.Models
{
    /// <summary>
    /// Класс для сериализации всех данных библиотеки в JSON
    /// </summary>
    public class LibraryData
    {
        /// <summary>
        /// Список всех книг
        /// </summary>
        public List<Book> Books { get; set; } = new List<Book>();

        /// <summary>
        /// Список всех читателей
        /// </summary>
        public List<Reader> Readers { get; set; } = new List<Reader>();

        /// <summary>
        /// Список всех выдач
        /// </summary>
        public List<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
    }
}

