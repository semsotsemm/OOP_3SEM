using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

public interface ILogger
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message, Exception ex = null);
}

internal enum LogType { INFO, WARNING, ERROR }

internal static class LoggerHelper
{
    public static string FormatMessage(LogType type, string message, Exception ex = null)
    {
        string timestamp = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");
        string logEntry = $"{timestamp}, {type}: {message}";

        if (ex != null)
        {
            logEntry += $"\n\t[Exception Details] Message: {ex.Message}" +
                        $"\n\t[Exception Type] {ex.GetType().FullName}" +
                        $"\n\t[Exception Source] {ex.StackTrace?.Trim().Split('\n').FirstOrDefault()}";
        }
        return logEntry;
    }
}

public class FileLogger : ILogger
{
    private readonly string _filePath;
    private static readonly object _lock = new object();

    public FileLogger(string filePath = "app_log.log")
    {
        _filePath = filePath;
    }

    public void LogInfo(string message) => WriteToFile(LogType.INFO, message);
    public void LogWarning(string message) => WriteToFile(LogType.WARNING, message);
    public void LogError(string message, Exception ex = null) => WriteToFile(LogType.ERROR, message, ex);

    private void WriteToFile(LogType type, string message, Exception ex = null)
    {
        lock (_lock)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(_filePath, true, Encoding.UTF8))
                {
                    sw.WriteLine(LoggerHelper.FormatMessage(type, message, ex));
                }
            }
            catch (Exception writeEx)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"FATAL LOGGER ERROR: Failed to write to log file {_filePath}. Error: {writeEx.Message}");
                Console.ResetColor();
            }
        }
    }
}

public class ConsoleLogger : ILogger
{
    public void LogInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(LoggerHelper.FormatMessage(LogType.INFO, message));
        Console.ResetColor();
    }

    public void LogWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(LoggerHelper.FormatMessage(LogType.WARNING, message));
        Console.ResetColor();
    }

    public void LogError(string message, Exception ex = null)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(LoggerHelper.FormatMessage(LogType.ERROR, message, ex));
        Console.ResetColor();
    }
}

public class SoftwareInstallationException : Exception
{
    public string SoftwareName { get; }

    public SoftwareInstallationException(string message, string softwareName) : base(message)
    {
        SoftwareName = softwareName;
    }
}

public class InvalidSoftwareDataException : ArgumentException
{
    public object AttemptedValue { get; }

    public InvalidSoftwareDataException(string paramName, string message, object attemptedValue)
        : base(message, paramName)
    {
        AttemptedValue = attemptedValue;
    }
}

public class SoftwareListIndexException : SystemException, ISerializable
{
    public int AttemptedIndex { get; }

    public SoftwareListIndexException(string message, int attemptedIndex) : base(message)
    {
        AttemptedIndex = attemptedIndex;
    }

    protected SoftwareListIndexException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        AttemptedIndex = info.GetInt32(nameof(AttemptedIndex));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(AttemptedIndex), AttemptedIndex);
    }
}

public interface IOperationSet
{
    bool ExecuteOperation(string operationName);
    void AddOperation(string operationName);
    string GetStatus();
}

public partial class OperationSet : IOperationSet
{
    private List<string> AvailableOperations = new List<string>();
    private bool IsOperationCompleted;

    public bool ExecuteOperation(string operationName) => true;
    public void AddOperation(string operationName) { }
    public string GetStatus() => "OK";
}

public struct Developer
{
    public string DeveloperName { get; }
    public string CompanyName { get; }

    public Developer(string developerName, string companyName)
    {
        DeveloperName = developerName;
        CompanyName = companyName;
    }

    public override string ToString()
    {
        return $"{DeveloperName} ({CompanyName})";
    }
}

public abstract partial class Software
{
    protected string _version;
    protected Developer _developer;

    public string Version { get => _version; set => _version = value; }
    public Developer Developer { get => _developer; set => _developer = value; }

    public abstract void Run();
}

public abstract partial class ApplicationSoftware : Software
{
    protected int _spentTime;
    protected LicenseType _licenseType;

    public int SpentTime { get => _spentTime; set => _spentTime = value; }
    public enum LicenseType
    {
        Free,
        Trial,
        Paid,
        Cracked
    }
    public LicenseType CurrentLicense { get { return _licenseType; } set { _licenseType = value; } }

    public override void Run()
    {
        Console.WriteLine($"{this.GetType().Name} запущено. Время использования: {_spentTime} мин.");
    }
}

public partial class WordProcessor : ApplicationSoftware
{
    protected IOperationSet Operations;
    protected string _currentDocumentPath;
}

public class Word : WordProcessor
{
    public Word(string version = "1.0")
    {
        _version = version;
        _developer = new Developer("Microsoft", "Corp");
        _licenseType = LicenseType.Paid;
    }

    public void InsertImage()
    {
        Console.WriteLine("Вставка изображения в документ...");
    }

    public override string ToString()
    {
        return $"[Word: v{_version}, Лицензия={_licenseType}]";
    }
}

public abstract partial class Game : ApplicationSoftware
{
    protected int _currentLevel;
    public int CurrentLevel { get => _currentLevel; set => _currentLevel = value; }
}

public sealed class Sapper : Game
{
    private int _mineCount;

    public int MineCount
    {
        get { return _mineCount; }
        set
        {
            if (value <= 0)
            {
                throw new InvalidSoftwareDataException(nameof(MineCount),
                    "Количество мин должно быть положительным.", value);
            }
            _mineCount = value;
        }
    }
    public Sapper()
    {
        MineCount = 10;
        CurrentLevel = 1;
        Version = "1.0";
        _developer = new Developer("Unknown", "Windows");
        _licenseType = LicenseType.Free;
    }
    public void CheckCell(int x, int y)
    {
        Console.WriteLine($"Проверяем ячейку ({x}, {y}).");
        SpentTime += 1;
    }

    public override string ToString()
    {
        return $"[Sapper: v{_version}, Кол-во мин={MineCount}]";
    }
}

public abstract class MaliciousSoftware : Software
{
    private bool _isDetected;

    public bool IsDetected
    {
        get { return _isDetected; }
        private set { _isDetected = value; }
    }

    public MaliciousSoftware(bool isDetected = false)
    {
        IsDetected = isDetected;
        Version = "1.0";
        Developer = new Developer("Хакер", "Алексей");
    }

    public override void Run()
    {
        if (!IsDetected)
        {
            Console.WriteLine($"Вредоносное ПО ({this.GetType().Name}) тайно запущено.");
            ExecutePayload();
        }
        else
        {
            Console.WriteLine($"Вредоносное ПО ({this.GetType().Name}) обнаружено и остановлено!");
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
        return $"[MaliciousSoftware: v{_version}, Обнаружено={IsDetected}]";
    }
}

public abstract class Virus : MaliciousSoftware
{
    public Virus(bool isDetected) : base(isDetected) { }
    public override string GetStatus() => "Virus: Spreading rapidly.";
    public override void ExecutePayload() => Console.WriteLine("Virus: Infecting files...");
}

public sealed class CConficker : Virus
{
    public CConficker() : base(isDetected: false)
    {
        Version = "A-2008";
    }

    public void PropagateViaNetwork()
    {
        Console.WriteLine($"CConficker: Используется сетевая ошибка...");
    }

    public override string ToString()
    {
        return $"[CConficker: v{_version}]";
    }
}

public class Computer
{
    private List<Software> _softwares;
    public int hardDrive;
    private readonly ILogger _logger;

    public List<Software> Softwares
    {
        get { return _softwares; }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value), "Список программ не может быть null");
            }

            _softwares = value;
            _logger.LogInfo("Список ПО был перезаписан.");
        }
    }

    public Computer(ILogger logger)
    {
        hardDrive = 1024;
        _softwares = new List<Software>();
        _logger = logger ?? new ConsoleLogger();
        _logger.LogInfo("Объект Computer создан.");
    }

    public bool Install(Software software)
    {
        if (software == null)
        {
            throw new ArgumentNullException(nameof(software), "Попытка установки null в качестве ПО.");
        }

        bool IsInstalled = _softwares.Any(s => s.GetType() == software.GetType());
        if (!IsInstalled)
        {
            _softwares.Add(software);
            _logger.LogInfo($"Программа '{software.GetType().Name}' устанавливается...");
        }
        else
        {
            throw new SoftwareInstallationException($"Программа '{software.GetType().Name}' уже установлена.", software.GetType().Name);
        }
        return !IsInstalled;
    }

    public Software GetSoftwareByIndex(int index)
    {
        if (index < 0 || index >= _softwares.Count)
        {
            throw new SoftwareListIndexException("Индекс выходит за пределы допустимого диапазона списка установленного ПО.", index);
        }
        return _softwares[index];
    }
}

public class ComputerController
{
    private readonly Computer _computer;
    private readonly ILogger _logger;

    public ComputerController(Computer computer, ILogger logger)
    {
        _computer = computer ?? throw new ArgumentNullException(nameof(computer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void LoadFromTextFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Файл конфигурации ПО не найден.", filePath);
        }

        foreach (var line in File.ReadLines(filePath))
        {
            Software software = CreateSoftwareByName(line.Trim());
            if (software != null)
            {
                _computer.Install(software);
            }
        }
        _logger.LogInfo($"ПО успешно загружено из {filePath}");
    }

    public void LoadFromJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("JSON файл не найден!", filePath);
        }

        var types = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(filePath));
        foreach (var type in types)
        {
            Software software = CreateSoftwareByName(type.Trim());
            if (software != null)
            {
                _computer.Install(software);
            }
        }
        _logger.LogInfo($"ПО успешно загружено из {filePath}");
    }

    private Software CreateSoftwareByName(string name)
    {
        return name switch
        {
            "Sapper" => new Sapper(),
            "Word" => new Word(),
            "CConficker" => new CConficker(),
            _ => null
        };
    }

    public List<Game> FindGames()
    {
        return _computer.Softwares
            .OfType<Game>()
            .ToList();
    }

    public Word FindTextEditor(string version)
    {
        Debug.Assert(version != null, "Version parameter cannot be null. This indicates a developer error.");

        return _computer.Softwares
            .OfType<Word>()
            .FirstOrDefault(word => word.Version == version);
    }

    public void printInAlphabeticalOrder()
    {
        if (_computer.Softwares.Count == 0)
        {
            _logger.LogInfo("Нет установленных программ.");
            return;
        }

        _logger.LogInfo("Программы в алфавитном порядке:");

        var sortedList = _computer.Softwares
            .OrderBy(software => software.GetType().Name)
            .ToList();

        for (int i = 0; i < sortedList.Count; i++)
        {
            Console.WriteLine($"{i + 1}: {sortedList[i]}");
        }
    }
}


class Test
{
    static void Main(string[] args)
    {
        ILogger logger = new ConsoleLogger();

        if (File.Exists("app_log.log"))
        {
            File.Delete("app_log.log");
        }

        PrepareFiles(logger);

        Computer computer = new Computer(logger);
        ComputerController controller = new ComputerController(computer, logger);

        logger.LogInfo("--- СТАРТ ОСНОВНОЙ ЛОГИКИ ---");

        try
        {
            controller.LoadFromTextFile("software.txt");
            controller.LoadFromJson("software.json");
            controller.printInAlphabeticalOrder();

            RunLaboratoryTests(logger, computer);
        }
        catch (InvalidSoftwareDataException ex)
        {
            logger.LogError("[ОШИБКА ДАННЫХ] Переданы неверные данные для ПО.", ex);
            Debug.WriteLine($"[Diagnostic] Параметр '{ex.ParamName}' получил недопустимое значение '{ex.AttemptedValue}'.");
        }
        catch (SoftwareListIndexException ex)
        {
            logger.LogError("[ОШИБКА ИНДЕКСА] Попытка доступа к ПО по неверному индексу.", ex);
            Debug.WriteLine($"[Diagnostic] Неверный индекс {ex.AttemptedIndex}.");
        }
        catch (SoftwareInstallationException ex)
        {
            logger.LogError("[ОШИБКА УСТАНОВКИ] Не удалось установить ПО.", ex);
            Debug.WriteLine($"[Diagnostic] Ошибка при установке '{ex.SoftwareName}'.");
        }
        catch (FileNotFoundException ex)
        {
            logger.LogError("[ОШИБКА ФАЙЛА] Необходимый файл не найден.", ex);
            Debug.WriteLine($"[Diagnostic] Проверьте наличие файла по пути: {ex.FileName}");
        }
        catch (ArgumentNullException ex)
        {
            logger.LogError("[NULL POINTER] Попытка использовать объект, который был null.", ex);
            Debug.WriteLine($"[Diagnostic] Параметр '{ex.ParamName}' не был инициализирован.");
        }
        catch (DivideByZeroException ex)
        {
            logger.LogError("[МАТЕМАТИЧЕСКАЯ ОШИБКА] Деление на ноль, проброшенное выше по стеку.", ex);
            Debug.WriteLine($"[Diagnostic] {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError("[НЕПРЕДВИДЕННАЯ ОШИБКА] Произошла неизвестная ошибка.", ex);
            Debug.WriteLine($"[Diagnostic] Необработанная ошибка: {ex.GetType().Name} - {ex.Message}");
        }

        finally
        {
            logger.LogInfo("--- ЗАВЕРШЕНИЕ ПРОГРАММЫ (Блок Finally) ---");
            Console.WriteLine("\nРабота программы завершена. Нажмите Enter для выхода.");
            Console.WriteLine("Проверьте файл 'app_log.log' для просмотра всех записей логгера.");
            Console.ReadLine();
        }
    }


    static void SimulateRethrow(ILogger logger)
    {
        try
        {
            int a = 10;
            int b = 0;
            int c = a / b;
        }
        catch (DivideByZeroException ex)
        {
            logger.LogWarning($"[Уровень 1] Поймана ошибка в SimulateRethrow. Пробрасываем дальше.");
            throw;
        }
    }

    static void RunLaboratoryTests(ILogger logger, Computer computer)
    {
        logger.LogWarning("\n=== НАЧАЛО ЛАБОРАТОРНЫХ ТЕСТОВ (5 исключений) ===");

        try
        {
            logger.LogInfo("ТЕСТ 1: Попытка установить 'Word' второй раз...");
            computer.Install(new Word("1.0"));
        }
        catch (SoftwareInstallationException ex)
        {
            logger.LogError("ТЕСТ 1 УСПЕШЕН: Поймано кастомное исключение SoftwareInstallationException.", ex);
            Debug.WriteLine($"[Diagnostic] Ошибка при установке '{ex.SoftwareName}'.");
        }
        catch (Exception ex) { logger.LogError("ТЕСТ 1 ПРОВАЛЕН: Поймано непредвиденное исключение.", ex); }

        try
        {
            logger.LogInfo("\nТЕСТ 2: Попытка задать MineCount = 0 для Sapper...");
            Sapper sapper = computer.Softwares.OfType<Sapper>().First();
            sapper.MineCount = 0;
        }
        catch (InvalidSoftwareDataException ex)
        {
            logger.LogError("ТЕСТ 2 УСПЕШЕН: Поймано кастомное исключение InvalidSoftwareDataException.", ex);
            Debug.WriteLine($"[Diagnostic] Параметр '{ex.ParamName}' получил недопустимое значение '{ex.AttemptedValue}'.");
        }
        catch (Exception ex) { logger.LogError("ТЕСТ 2 ПРОВАЛЕН: Поймано непредвиденное исключение.", ex); }

        try
        {
            logger.LogInfo("\nТЕСТ 3: Попытка доступа к элементу по индексу 99...");
            computer.GetSoftwareByIndex(99);
        }
        catch (SoftwareListIndexException ex)
        {
            logger.LogError("ТЕСТ 3 УСПЕШЕН: Поймано кастомное исключение SoftwareListIndexException.", ex);
            Debug.WriteLine($"[Diagnostic] Неверный индекс {ex.AttemptedIndex}.");
        }
        catch (Exception ex) { logger.LogError("ТЕСТ 3 ПРОВАЛЕН: Поймано непредвиденное исключение.", ex); }

        try
        {
            logger.LogInfo("\nТЕСТ 4: Попытка установить список Softwares в null...");
            computer.Softwares = null;
        }
        catch (ArgumentNullException ex)
        {
            logger.LogError("ТЕСТ 4 УСПЕШЕН: Поймано стандартное исключение ArgumentNullException.", ex);
            Debug.WriteLine($"[Diagnostic] Параметр '{ex.ParamName}' не был инициализирован.");
        }
        catch (Exception ex) { logger.LogError("ТЕСТ 4 ПРОВАЛЕН: Поймано непредвиденное исключение.", ex); }

        logger.LogWarning("\nТЕСТ 5: Демонстрация проброса исключения (SimulateRethrow)");
        try
        {
            SimulateRethrow(logger);
        }
        catch (DivideByZeroException ex)
        {
            logger.LogError("ТЕСТ 5 УСПЕШЕН: Поймано DivideByZeroException после проброса.", ex);
            Debug.WriteLine($"[Diagnostic] Проброс сработал: {ex.Message}");
        }
        catch (Exception ex) { logger.LogError("ТЕСТ 5 ПРОВАЛЕН: Поймано непредвиденное исключение.", ex); }

        logger.LogWarning("=== ЛАБОРАТОРНЫЕ ТЕСТЫ ЗАВЕРШЕНЫ ===");
    }

    static void PrepareFiles(ILogger logger)
    {
        try
        {
            File.WriteAllLines("software.txt", new string[] { "Word", "Sapper", "CConficker" });
            File.WriteAllText("software.json", JsonConvert.SerializeObject(new List<string> { "Sapper", "Word" }, Formatting.Indented));
        }
        catch (IOException ex)
        {
            logger.LogError("Не удалось подготовить файлы конфигурации.", ex);
            throw new Exception("Критическая ошибка: не удалось создать файлы.", ex);
        }
    }
}