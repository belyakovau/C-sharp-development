using System;
using System.IO;
using System.Linq;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager
{
    /// <summary>
    /// Главный класс программы
    /// Отвечает за пользовательский интерфейс и взаимодействие с пользователем
    /// </summary>
    class Program
    {
        // Путь к файлу данных (относительно исполняемого файла)
        private static readonly string DataFilePath = GetDataFilePath();

        /// <summary>
        /// Получает полный путь к файлу данных, создавая папку Data при необходимости
        /// </summary>
        private static string GetDataFilePath()
        {
            // Получаем директорию, где находится исполняемый файл
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            
            // Создаем путь к папке Data
            string dataDirectory = Path.Combine(baseDirectory, "Data");
            
            // Создаем папку Data, если её нет
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
            
            // Возвращаем полный путь к файлу
            return Path.Combine(dataDirectory, "tasks.json");
        }

        /// <summary>
        /// Точка входа в программу
        /// </summary>
        static void Main(string[] args)
        {
            // Создаем экземпляр сервиса для управления задачами
            TaskManagerService taskManager = new TaskManagerService(DataFilePath);

            // Приветственное сообщение
            Console.WriteLine("╔══════════════════════════════════╗");
            Console.WriteLine("║   ДОБРО ПОЖАЛОВАТЬ В МЕНЕДЖЕР    ║");
            Console.WriteLine("║         ЗАДАЧ (TODO LIST)        ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            // Главный цикл программы
            while (true)
            {
                // Показываем меню
                ShowMainMenu();

                // Читаем выбор пользователя
                string? choice = Console.ReadLine();

                // Обрабатываем выбор
                switch (choice)
                {
                    case "1":
                        HandleAddTask(taskManager);
                        break;
                    case "2":
                        HandleShowAllTasks(taskManager);
                        break;
                    case "3":
                        HandleUpdateTask(taskManager);
                        break;
                    case "4":
                        HandleDeleteTask(taskManager);
                        break;
                    case "5":
                        HandleSearchTasks(taskManager);
                        break;
                    case "6":
                        HandleFilterTasks(taskManager);
                        break;
                    case "7":
                        HandleSortTasks(taskManager);
                        break;
                    case "8":
                        taskManager.ShowStatistics();
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
            Console.WriteLine("1. Добавить задачу");
            Console.WriteLine("2. Показать все задачи");
            Console.WriteLine("3. Обновить задачу");
            Console.WriteLine("4. Удалить задачу");
            Console.WriteLine("5. Поиск задач");
            Console.WriteLine("6. Фильтрация задач");
            Console.WriteLine("7. Сортировка задач");
            Console.WriteLine("8. Статистика");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите действие: ");
        }

        /// <summary>
        /// Обрабатывает добавление новой задачи
        /// </summary>
        static void HandleAddTask(TaskManagerService taskManager)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║        ДОБАВЛЕНИЕ ЗАДАЧИ         ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.Write("Название задачи: ");
            string? title = Console.ReadLine();

            Console.Write("Описание (необязательно): ");
            string? description = Console.ReadLine();

            Console.Write("Приоритет (Low/Medium/High) [по умолчанию: Medium]: ");
            string? priority = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(priority))
            {
                priority = "Medium";
            }

            Console.Write("Дата выполнения (dd.MM.yyyy, необязательно): ");
            string? dueDateInput = Console.ReadLine();
            DateTime? dueDate = null;
            if (!string.IsNullOrWhiteSpace(dueDateInput))
            {
                if (DateTime.TryParse(dueDateInput, out DateTime parsedDate))
                {
                    dueDate = parsedDate;
                }
                else
                {
                    Console.WriteLine("⚠ Неверный формат даты. Дата выполнения не установлена.");
                }
            }

            taskManager.AddTask(title ?? "", description ?? "", priority, dueDate);
        }

        /// <summary>
        /// Обрабатывает отображение всех задач
        /// </summary>
        static void HandleShowAllTasks(TaskManagerService taskManager)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║         ВСЕ ЗАДАЧИ               ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            var tasks = taskManager.GetAllTasks();

            if (tasks.Count == 0)
            {
                Console.WriteLine("Задач пока нет. Добавьте первую задачу!");
            }
            else
            {
                foreach (var task in tasks)
                {
                    Console.WriteLine(task);
                }
            }
        }

        /// <summary>
        /// Обрабатывает обновление задачи
        /// </summary>
        static void HandleUpdateTask(TaskManagerService taskManager)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║        ОБНОВЛЕНИЕ ЗАДАЧИ          ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.Write("Введите ID задачи для обновления: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var task = taskManager.GetTaskById(id);
                if (task == null)
                {
                    Console.WriteLine($"✗ Задача с ID {id} не найдена");
                    return;
                }

                Console.WriteLine($"\nТекущая задача: {task.Title}");
                Console.Write("Новое название (Enter - оставить без изменений): ");
                string? newTitle = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newTitle))
                {
                    newTitle = task.Title;
                }

                Console.Write("Новое описание (Enter - оставить без изменений): ");
                string? newDescription = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newDescription))
                {
                    newDescription = task.Description;
                }

                Console.Write("Выполнена? (true/false, Enter - оставить без изменений): ");
                string? completedInput = Console.ReadLine();
                bool newIsCompleted = task.IsCompleted;
                if (!string.IsNullOrWhiteSpace(completedInput))
                {
                    if (bool.TryParse(completedInput, out bool parsed))
                    {
                        newIsCompleted = parsed;
                    }
                }

                Console.Write("Приоритет (Low/Medium/High, Enter - оставить без изменений): ");
                string? newPriority = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newPriority))
                {
                    newPriority = task.Priority;
                }

                taskManager.UpdateTask(id, newTitle, newDescription, newIsCompleted, newPriority);
            }
            else
            {
                Console.WriteLine("✗ Неверный формат ID");
            }
        }

        /// <summary>
        /// Обрабатывает удаление задачи
        /// </summary>
        static void HandleDeleteTask(TaskManagerService taskManager)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║         УДАЛЕНИЕ ЗАДАЧИ          ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.Write("Введите ID задачи для удаления: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                taskManager.DeleteTask(id);
            }
            else
            {
                Console.WriteLine("✗ Неверный формат ID");
            }
        }

        /// <summary>
        /// Обрабатывает поиск задач
        /// </summary>
        static void HandleSearchTasks(TaskManagerService taskManager)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║          ПОИСК ЗАДАЧ              ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.Write("Введите поисковый запрос: ");
            string? searchTerm = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Console.WriteLine("✗ Поисковый запрос не может быть пустым");
                return;
            }

            var results = taskManager.SearchTasks(searchTerm);

            if (results.Count == 0)
            {
                Console.WriteLine($"Не найдено задач по запросу '{searchTerm}'");
            }
            else
            {
                Console.WriteLine($"\nНайдено задач: {results.Count}");
                foreach (var task in results)
                {
                    Console.WriteLine(task);
                }
            }
        }

        /// <summary>
        /// Обрабатывает фильтрацию задач
        /// </summary>
        static void HandleFilterTasks(TaskManagerService taskManager)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║       ФИЛЬТРАЦИЯ ЗАДАЧ           ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.WriteLine("Выберите фильтр:");
            Console.WriteLine("1. Выполненные задачи");
            Console.WriteLine("2. Невыполненные задачи");
            Console.WriteLine("3. Все задачи");
            Console.Write("Ваш выбор: ");

            string? choice = Console.ReadLine();
            bool? filter = null;

            switch (choice)
            {
                case "1":
                    filter = true;
                    break;
                case "2":
                    filter = false;
                    break;
                case "3":
                    filter = null;
                    break;
                default:
                    Console.WriteLine("✗ Неверный выбор");
                    return;
            }

            var filteredTasks = taskManager.FilterTasks(filter);

            if (filteredTasks.Count == 0)
            {
                Console.WriteLine("Задач не найдено");
            }
            else
            {
                Console.WriteLine($"\nНайдено задач: {filteredTasks.Count}");
                foreach (var task in filteredTasks)
                {
                    Console.WriteLine(task);
                }
            }
        }

        /// <summary>
        /// Обрабатывает сортировку задач
        /// </summary>
        static void HandleSortTasks(TaskManagerService taskManager)
        {
            Console.WriteLine("\n╔══════════════════════════════════╗");
            Console.WriteLine("║        СОРТИРОВКА ЗАДАЧ           ║");
            Console.WriteLine("╚══════════════════════════════════╝");

            Console.WriteLine("Выберите критерий сортировки:");
            Console.WriteLine("1. По дате создания");
            Console.WriteLine("2. По приоритету");
            Console.WriteLine("3. По названию");
            Console.Write("Ваш выбор: ");

            string? choice = Console.ReadLine();
            string sortBy = choice switch
            {
                "1" => "date",
                "2" => "priority",
                "3" => "title",
                _ => "date"
            };

            var sortedTasks = taskManager.SortTasks(sortBy);

            Console.WriteLine($"\nЗадачи отсортированы по: {sortBy}");
            foreach (var task in sortedTasks)
            {
                Console.WriteLine(task);
            }
        }
    }
}