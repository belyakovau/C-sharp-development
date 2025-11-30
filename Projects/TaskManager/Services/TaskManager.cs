using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TaskManager.Models;

namespace TaskManager.Services
{
    /// <summary>
    /// Сервис для управления задачами
    /// Отвечает за CRUD операции и работу с файлом данных
    /// </summary>
    public class TaskManagerService
    {
        // Приватные поля класса
        private List<TaskItem> _tasks;           // Список всех задач в памяти
        private string _filePath;                // Путь к JSON файлу
        private int _nextId;                     // Следующий доступный ID

        /// <summary>
        /// Конструктор класса TaskManagerService
        /// Инициализирует список задач и загружает данные из файла
        /// </summary>
        /// <param name="filePath">Путь к JSON файлу для сохранения данных</param>
        public TaskManagerService(string filePath)
        {
            // Сохраняем путь к файлу
            this._filePath = filePath;

            // Инициализируем пустой список задач
            this._tasks = new List<TaskItem>();

            // Начинаем ID с 1
            this._nextId = 1;

            // Загружаем задачи из файла при создании объекта
            LoadTasks();
        }

        /// <summary>
        /// Загружает задачи из JSON файла в память
        /// </summary>
        public void LoadTasks()
        {
            try
            {
                // Проверяем, существует ли файл
                if (File.Exists(_filePath))
                {
                    // Читаем весь текст из файла
                    string json = File.ReadAllText(_filePath);

                    // Десериализуем JSON в список задач
                    // Если файл пустой или некорректный, используем пустой список
                    _tasks = JsonSerializer.Deserialize<List<TaskItem>>(json)
                        ?? new List<TaskItem>();

                    // Если есть задачи, находим максимальный ID и устанавливаем nextId
                    if (_tasks.Count > 0)
                    {
                        _nextId = _tasks.Max(t => t.Id) + 1;
                    }
                }
                else
                {
                    // Если файла нет, создаем пустой список
                    _tasks = new List<TaskItem>();
                }
            }
            catch (Exception ex)
            {
                // Если произошла ошибка (некорректный JSON, нет прав доступа и т.д.)
                Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}");
                _tasks = new List<TaskItem>();
            }
        }

        /// <summary>
        /// Сохраняет задачи из памяти в JSON файл
        /// </summary>
        public void SaveTasks()
        {
            try
            {
                // Получаем директорию файла
                string? directory = Path.GetDirectoryName(_filePath);
                
                // Создаем директорию, если её нет
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Настройки для сериализации (красивый формат с отступами)
                JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true  // Делает JSON читаемым
                };

                // Сериализуем список задач в JSON строку
                string json = JsonSerializer.Serialize(_tasks, options);

                // Записываем JSON в файл (перезаписывает файл, если он существует)
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                // Если произошла ошибка (нет прав доступа, диск переполнен и т.д.)
                Console.WriteLine($"Ошибка при сохранении задач: {ex.Message}");
            }
        }

        /// <summary>
        /// Создает новую задачу и добавляет ее в список
        /// </summary>
        /// <param name="title">Название задачи</param>
        /// <param name="description">Описание задачи</param>
        /// <param name="priority">Приоритет: "Low", "Medium", "High"</param>
        /// <param name="dueDate">Опциональная дата выполнения</param>
        public void AddTask(string title, string description, string priority, DateTime? dueDate = null)
        {
            // Валидация входных данных
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Ошибка: Название задачи не может быть пустым!");
                return;
            }

            // Создаем новый объект задачи
            TaskItem newTask = new TaskItem
            {
                Id = _nextId++,                    // Присваиваем уникальный ID и увеличиваем счетчик
                Title = title.Trim(),               // Убираем лишние пробелы
                Description = description?.Trim() ?? string.Empty,  // Описание может быть пустым
                Priority = priority ?? "Medium",    // По умолчанию "Medium"
                IsCompleted = false,                // Новая задача не выполнена
                CreatedDate = DateTime.Now,         // Текущая дата и время
                DueDate = dueDate                   // Опциональная дата выполнения
            };

            // Добавляем задачу в список
            _tasks.Add(newTask);

            // Сохраняем изменения в файл
            SaveTasks();

            Console.WriteLine($"✓ Задача '{title}' добавлена (ID: {newTask.Id})");
        }

        /// <summary>
        /// Возвращает все задачи
        /// </summary>
        /// <returns>Список всех задач</returns>
        public List<TaskItem> GetAllTasks()
        {
            return _tasks;
        }

        /// <summary>
        /// Находит задачу по ID
        /// </summary>
        /// <param name="id">ID задачи</param>
        /// <returns>Задача или null, если не найдена</returns>
        public TaskItem? GetTaskById(int id)
        {
            // LINQ: FirstOrDefault возвращает первую задачу с указанным ID или null
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// Обновляет существующую задачу
        /// </summary>
        /// <param name="id">ID задачи для обновления</param>
        /// <param name="title">Новое название</param>
        /// <param name="description">Новое описание</param>
        /// <param name="isCompleted">Новый статус выполнения</param>
        /// <param name="priority">Новый приоритет</param>
        public void UpdateTask(int id, string title, string description, bool isCompleted, string priority)
        {
            // Находим задачу по ID
            TaskItem? task = GetTaskById(id);

            if (task != null)
            {
                // Обновляем свойства задачи
                task.Title = title.Trim();
                task.Description = description?.Trim() ?? string.Empty;
                task.IsCompleted = isCompleted;
                task.Priority = priority ?? "Medium";

                // Сохраняем изменения в файл
                SaveTasks();

                Console.WriteLine($"✓ Задача (ID: {id}) обновлена");
            }
            else
            {
                Console.WriteLine($"✗ Задача с ID {id} не найдена");
            }
        }

        /// <summary>
        /// Удаляет задачу по ID
        /// </summary>
        /// <param name="id">ID задачи для удаления</param>
        public void DeleteTask(int id)
        {
            // Находим задачу по ID
            TaskItem? task = GetTaskById(id);

            if (task != null)
            {
                // Удаляем задачу из списка
                _tasks.Remove(task);

                // Сохраняем изменения в файл
                SaveTasks();

                Console.WriteLine($"✓ Задача '{task.Title}' удалена");
            }
            else
            {
                Console.WriteLine($"✗ Задача с ID {id} не найдена");
            }
        }

        /// <summary>
        /// Ищет задачи по названию или описанию
        /// </summary>
        /// <param name="searchTerm">Поисковый запрос</param>
        /// <returns>Список найденных задач</returns>
        public List<TaskItem> SearchTasks(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<TaskItem>();
            }

            // LINQ: Where фильтрует задачи, где название или описание содержит поисковый запрос
            // StringComparison.OrdinalIgnoreCase делает поиск без учета регистра
            return _tasks
                .Where(t =>
                    t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    t.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Фильтрует задачи по статусу выполнения
        /// </summary>
        /// <param name="isCompleted">true - выполненные, false - невыполненные, null - все</param>
        /// <returns>Отфильтрованный список задач</returns>
        public List<TaskItem> FilterTasks(bool? isCompleted)
        {
            if (isCompleted.HasValue)
            {
                // LINQ: Where фильтрует задачи по статусу
                return _tasks.Where(t => t.IsCompleted == isCompleted.Value).ToList();
            }

            // Если фильтр не указан, возвращаем все задачи
            return _tasks;
        }

        /// <summary>
        /// Сортирует задачи по указанному критерию
        /// </summary>
        /// <param name="sortBy">Критерий сортировки: "date", "priority", "title"</param>
        /// <returns>Отсортированный список задач</returns>
        public List<TaskItem> SortTasks(string sortBy)
        {
            switch (sortBy.ToLower())
            {
                case "date":
                    // Сортировка по дате создания (от старых к новым)
                    return _tasks.OrderBy(t => t.CreatedDate).ToList();

                case "priority":
                    // Словарь для определения порядка приоритетов
                    var priorityOrder = new Dictionary<string, int>
                    {
                        { "High", 1 },
                        { "Medium", 2 },
                        { "Low", 3 }
                    };
                    // Сортировка по приоритету (High → Medium → Low)
                    return _tasks.OrderBy(t =>
                        priorityOrder.GetValueOrDefault(t.Priority, 4)).ToList();

                case "title":
                    // Сортировка по названию (алфавитный порядок)
                    return _tasks.OrderBy(t => t.Title).ToList();

                default:
                    // Если критерий не указан, возвращаем без сортировки
                    return _tasks;
            }
        }

        /// <summary>
        /// Возвращает статистику по задачам
        /// </summary>
        public void ShowStatistics()
        {
            int total = _tasks.Count;
            int completed = _tasks.Count(t => t.IsCompleted);
            int pending = total - completed;

            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║         СТАТИСТИКА ЗАДАЧ         ║");
            Console.WriteLine("╚══════════════════════════════════╝");
            Console.WriteLine($"Всего задач: {total}");
            Console.WriteLine($"Выполнено: {completed}");
            Console.WriteLine($"Не выполнено: {pending}");

            if (total > 0)
            {
                double completionRate = (double)completed / total * 100;
                Console.WriteLine($"Процент выполнения: {completionRate:F1}%");
            }
        }
    }
}