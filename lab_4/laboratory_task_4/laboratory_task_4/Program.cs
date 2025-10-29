using System;
using System.Collections.Generic;

public interface IOperationSet
{
    bool ExecuteOperation(string operationName);
    void AddOperation(string operationName);
    string GetStatus();
}

public class OperationSet : IOperationSet
{
    private List<string> AvailableOperations = new List<string>();
    private bool IsOperationCompleted;

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

public class Developer
{
    private string _developerName;
    private string _companyName;

    public string CompanyName
    {
        get
        {
            return _companyName;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                Console.WriteLine("Название компании не может отсутствовать");
                return;
            }
            else
            {
                _companyName = value;
            }
        }
    }

    public string DeveloperName
    {
        get
        {
            return _developerName;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                Console.WriteLine("Имя разработчика не может отсутствовать");
                return;
            }
            else
            {
                _developerName = value;
            }
        }
    }

    public Developer() : this("Алексей", "БГТУ") { }

    public Developer(string name, string company)
    {
        DeveloperName = name;
        CompanyName = company;
    }

    public override string ToString()
    {
        return $"[Разработчик: {DeveloperName}, Компания: {CompanyName}]";
    }

    public override bool Equals(object obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
        {
            return false;
        }
        else
        {
            Developer other = (Developer)obj;
            return DeveloperName == other.DeveloperName && CompanyName == other.CompanyName;
        }
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DeveloperName, CompanyName);
    }

    public static bool operator ==(Developer left, Developer right)
    {
        if (ReferenceEquals(left, null))
        {
            return ReferenceEquals(right, null);
        }
        return left.Equals(right);
    }

    public static bool operator !=(Developer left, Developer right)
    {
        return !(left == right);
    }
}

public class Software
{
    private string _version;
    private Developer _developer;

    public Developer Developer
    {
        get { return _developer; }
        set
        {
            if (value == null || string.IsNullOrEmpty(value.DeveloperName) || string.IsNullOrEmpty(value.CompanyName))
            {
                Console.WriteLine("Разработчик ПО должен быть определен");
                return;
            }
            _developer = value;
        }
    }
    public string Version
    {
        get { return _version; }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                Console.WriteLine("Название версии ПО не может отсутствовать");
                return;
            }
            _version = value;
        }
    }

    public Software()
    {
        _version = "11.7";
        _developer = new Developer();
    }

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

public class ApplicationSoftware : Software
{
    private int _spentTime;
    private bool _isLicensed;

    public bool IsLicensed
    {
        get
        {
            return _isLicensed;
        }
        set
        {
            _isLicensed = value;
        }
    }

    public int SpentTime
    {
        get
        {
            return _spentTime;
        }
        set
        {
            if (value < 0)
            {
                Console.WriteLine("Проведенное время не может быть отрицательным");
                return;
            }
            _spentTime = value;
        }
    }
    public override void Run()
    {
        Console.WriteLine($"ПО запущено. Лицензия: {(IsLicensed ? "Активна" : "Отсутствует")}.");
    }
    public ApplicationSoftware()
    {
        _spentTime = 0;
        _isLicensed = true;
    }

    public void Activate(string key)
    {
        if (!string.IsNullOrEmpty(key) && key.Length > 5)
        {
            IsLicensed = true;
            Console.WriteLine("Приложение успешно активировано.");
        }
        else
        {
            Console.WriteLine("Неверный ключ активации.");
        }
    }

    public override string ToString()
    {
        return $"[ApplicationSoftware: (База: {base.ToString()}), Время={SpentTime}, Лицензия={IsLicensed}]";
    }
}

public class WordProcessor : ApplicationSoftware
{
    private readonly IOperationSet Operations;
    private string _currentDocumentPath;

    public string CurrentDocumentPath
    {
        get
        {
            return _currentDocumentPath;
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                Console.WriteLine("Путь к документу не может быть пустым.");
                return;
            }
            _currentDocumentPath = value;
        }
    }

    public WordProcessor()
    {
        Operations = new OperationSet();
        Operations.AddOperation("Открыть файл");
        Operations.AddOperation("Сохранить документ");
        Operations.AddOperation("Форматировать текст");
        Console.WriteLine("Текстовый процессор инициализирован с набором операций.");
        _currentDocumentPath = "НовыйДокумент.docx";
    }

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

public class Word : WordProcessor
{
    public void InsertImage()
    {
        Console.WriteLine("Вставка изображения в документ...");
    }

    public override string ToString()
    {
        return $"[Word: (База: {base.ToString()})]";
    }
}

public class Game : ApplicationSoftware
{
    private int _currentLevel;

    public int CurrentLevel
    {
        get
        {
            return _currentLevel;
        }
        set
        {
            if (value < 1)
            {
                Console.WriteLine("Уровень должен быть >= 1.");
                return;
            }
            _currentLevel = value;
        }
    }

    public Game()
    {
        _currentLevel = 1;
    }

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

public sealed class Sapper : Game
{
    private int _mineCount;

    public int MineCount
    {
        get
        {
            return _mineCount;
        }
        set
        {
            if (value <= 0)
            {
                Console.WriteLine("Количество мин должно быть положительным.");
                return;
            }
            _mineCount = value;
        }
    }

    public Sapper()
    {
        MineCount = 10;
        CurrentLevel = 1;
    }

    public void CheckCell(int x, int y)
    {
        Console.WriteLine($"Проверяем ячейку ({x}, {y}).");
        SpentTime += 1;
    }

    public override string ToString()
    {
        return $"[Sapper: (База: {base.ToString()}), Кол-во мин={MineCount}]";
    }
}

public abstract class MaliciousSoftware : Software
{
    private bool _isDetected;

    public bool IsDetected
    {
        get
        {
            return _isDetected;
        }
        private set
        {
            _isDetected = value;
        }
    }

    public MaliciousSoftware()
    {
        IsDetected = false;
        Version = "1.0";
        Developer = new Developer("Хакер", "Алексей");
    }

    public override void Run()
    {
        if (!IsDetected)
        {
            Console.WriteLine("Вредоносное ПО тайно запущено.");
            ExecutePayload();
        }
        else
        {
            Console.WriteLine("Вредоносное ПО обнаружено и остановлено!");
        }
    }

    public abstract void ExecutePayload();

    public void EvadeAntivirus()
    {
        Console.WriteLine("Попытка избежать обнаружения антивирусом...");
    }
    public abstract string GetStatus();

    public override string ToString()
    {
        return $"[MaliciousSoftware: (База: {base.ToString()}), Обнаружено={IsDetected}]";
    }
}

public class Virus : MaliciousSoftware
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

public sealed class CConficker : Virus
{
    public CConficker()
    {
        Version = "A-2008";
    }

    public void PropagateViaNetwork()
    {
        Console.WriteLine($"CConficker: Используется сетевая ошибка...");
    }

    public override string ToString()
    {
        return $"[CConficker: (База: {base.ToString()})]";
    }
}

public class Printer
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


class Program
{
    static void Main(string[] args)
    {

        Software appWord = new Word();
        Software appSapper = new Sapper();

        MaliciousSoftware virus = new CConficker();

        IOperationSet operations = new OperationSet();

        List<Software> softwareList = new List<Software> { appWord, appSapper, virus };

        Console.WriteLine("1. Демонстрация полиморфизма");

        foreach (var item in softwareList)
        {
            Console.WriteLine($"\n--- Работа c {item.GetType().Name} ---");

            item.Run();

            item.CheckForUpdates();


            if (item is ApplicationSoftware application)
            {
                Console.WriteLine($"Время использования (SpentTime): {application.SpentTime} мин.");
            }

            if (item is WordProcessor wordProc)
            {
                Console.WriteLine($"Документ: {wordProc.CurrentDocumentPath}. Статус: {wordProc.GetStatusDescriprion()}");
            }

            MaliciousSoftware malicious = item as MaliciousSoftware;
            if (malicious != null)
            {
                Console.WriteLine($"Вредоносное ПО. Обнаружено: {malicious.IsDetected}");
                malicious.EvadeAntivirus();
                Console.WriteLine($"Статус: {malicious.GetStatus()}");
            }
        }

        Console.WriteLine("\n3. Доступ к методам с помощью as");

        CConficker conficker = virus as CConficker;
        if (conficker != null)
        {
            Console.Write("Вызов уникального метода CConficker: ");
            conficker.PropagateViaNetwork();
        }

        Word myWord = appWord as Word;
        if (myWord != null)
        {
            Console.Write("Вызов уникального метода Word: ");
            myWord.InsertImage();
        }

        Console.WriteLine("\n4. Работа через интерфейс");

        operations.AddOperation("Печать");
        operations.ExecuteOperation("Печать");
        operations.ExecuteOperation("Неизвестная операция");
        Console.WriteLine(operations.GetStatus());

        Console.WriteLine("7. Демонстрация класса Printer");

        Software[] hierarchyArray = new Software[]
        {
            new Software(),
            new ApplicationSoftware(),
            new WordProcessor(),
            appWord,
            new Game(),
            appSapper,
            virus,
            new CConficker()
        };

        Printer printer = new Printer();

        foreach (Software obj in hierarchyArray)
        {
            printer.IAmPrinting(obj);
        }

        Console.WriteLine("\nДемонстрация завершена.");
    }
}