    public partial class Software
    {
        public virtual void Run()
        {
            Console.WriteLine($"ПО (Версия: {Version}, Автор: {Developer.DeveloperName}) запущено.");
        }

        public void CheckForUpdates()
        {
            Console.WriteLine($"Проверка обновлений для версии {Version}...");
        }

        public override string ToString()
        {
            return $"[Software: Версия={Version}, Разработчик={Developer.DeveloperName}]";
        }
    }

    public partial class WordProcessor : ApplicationSoftware
    {
        public void PerformSave()
        {
            if (Operations.ExecuteOperation("Сохранить документ"))
            {
                Console.WriteLine($"Документ сохранен в: {CurrentDocumentPath}");
            }
        }

        public void OpenFile(string path)
        {
            if (Operations.ExecuteOperation("Открыть файл"))
            {
                CurrentDocumentPath = path;
                Console.WriteLine($"Открыт документ: {CurrentDocumentPath}");
            }
        }
        public string GetStatusDescriprion()
        {
            return Operations.GetStatus();
        }

        public override string ToString()
        {
            return $"[WordProcessor: (База: {base.ToString()}), Документ={CurrentDocumentPath}, {Operations.GetStatus()}]";
        }
    }

    public partial class Game : ApplicationSoftware
    {
        public override void Run()
        {
            Console.WriteLine($"Игра запущена. Текущий уровень: {CurrentLevel}.");
        }

        public void StartNewGame()
        {
            CurrentLevel = 1;
            Console.WriteLine("Начата новая игра.");
        }

        public override string ToString()
        {
            return $"[Game: (База: {base.ToString()}), Уровень={CurrentLevel}]";
        }
    }

    public partial class Virus : MaliciousSoftware
    {
        public override void ExecutePayload()
        {
            Console.WriteLine("Вирус: Начинается заражение файлов.");
        }
        public void Replicate()
        {
            Console.WriteLine("Вирус успешно создал свою копию.");
        }
        public override string GetStatus()
        {
            return $"Вирус ({Version}): Текущий статус — {(IsDetected ? "Остановлен" : "Активен")}.";
        }

        public override string ToString()
        {
            return $"[Virus: (База: {base.ToString()})]";
        }
    }

    public partial class Printer
    {
        public void IAmPrinting(Software someobj)
        {
            if (someobj == null)
            {
                Console.WriteLine("Передан null-объект.");
                return;
            }
            Console.WriteLine($"\nОбъект типа: {someobj.GetType().Name}");
            Console.WriteLine(someobj.ToString());
        }
    }

    public partial class OperationSet : IOperationSet
    {
        public void AddOperation(string operationName)
        {
            AvailableOperations.Add(operationName);
            Console.WriteLine($"Операция '{operationName}' добавлена.");
        }

        public bool ExecuteOperation(string operationName)
        {
            if (AvailableOperations.Contains(operationName))
            {
                Console.WriteLine($"Выполняется операция: {operationName}");
                IsOperationCompleted = true;
            }
            else
            {
                Console.WriteLine($"Ошибка: Операция '{operationName}' не найдена.");
                IsOperationCompleted = false;
            }
            return IsOperationCompleted;
        }
        public string GetStatus()
        {
            return $"Набор операций содержит {AvailableOperations.Count} доступных операций.";
        }

        public override string ToString()
        {
            return $"[OperationSet: Доступно операций={AvailableOperations.Count}, Последняя операция завершена={IsOperationCompleted}]";
        }
    }