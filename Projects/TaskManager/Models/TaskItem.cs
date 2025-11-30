using System;

namespace TaskManager.Models
{
    /// <summary>
    /// Модель задачи (Task Item)
    /// Представляет одну задачу в системе управления задачами
    /// </summary>
    public class TaskItem
    {
        /// <summary>
        /// Уникальный идентификатор задачи
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название задачи
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Описание задачи
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Статус выполнения задачи
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Дата и время создания задачи
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Опциональная дата выполнения (может быть null)
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Приоритет задачи: "Low", "Medium", "High"
        /// </summary>
        public string Priority { get; set; } = "Medium";

        /// <summary>
        /// Переопределение метода ToString() для удобного вывода
        /// </summary>
        public override string ToString()
        {
            string status = IsCompleted ? "✓ Выполнена" : "○ Не выполнена";
            string dueDateInfo = DueDate.HasValue
                ? $" | Срок: {DueDate.Value:dd.MM.yyyy}"
                : "";

            return $"ID: {Id} | {Title} | {status} | Приоритет: {Priority}{dueDateInfo}";
        }
    }
}