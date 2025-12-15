using System;
using System.Collections.Generic;
using System.Xml.Serialization; // Для XML
using System.Text.Json.Serialization; // Для JSON
using System.Runtime.Serialization; // Для Binary/SOAP

// --- Классы из ЛР №4, адаптированные для сериализации ---

#region Интерфейс и вспомогательные классы

public interface IOperationSet
{
    // Методы должны быть public, но в IOperationSet они таковыми являются по умолчанию
    bool ExecuteOperation(string operationName);
    void AddOperation(string operationName);
    string GetStatus();
}

[Serializable] // Добавлен для сериализации
public class OperationSet : IOperationSet
{
    // Сделано публичным для JSON/XML сериализации
    public List<string> AvailableOperations { get; set; } = new List<string>();

    // Сделано публичным для JSON/XML сериализации
    public bool IsOperationCompleted { get; set; }

    // Пустой конструктор для сериализации
    public OperationSet() { }

    public void AddOperation(string operationName)
    {
        AvailableOperations.Add(operationName);
    }

    public bool ExecuteOperation(string operationName)
    {
        if (AvailableOperations.Contains(operationName))
        {
            IsOperationCompleted = true;
        }
        else
        {
            IsOperationCompleted = false;
        }
        return IsOperationCompleted;
    }

    public string GetStatus()
    {
        return $"Набор операций содержит {AvailableOperations.Count} доступных операций.";
    }
}

[Serializable]
public class Developer
{
    private string _developerName;
    private string _companyName;

    public string CompanyName
    {
        get { return _companyName; }
        set { _companyName = value; }
    }

    public string DeveloperName
    {
        get { return _developerName; }
        set { _developerName = value; }
    }

    // Пустой конструктор обязателен для сериализации
    public Developer() { }

    public Developer(string name, string company)
    {
        DeveloperName = name;
        CompanyName = company;
    }

    public override string ToString() => $"[Разработчик: {DeveloperName}, Компания: {CompanyName}]";
}

#endregion

#region Иерархия Software

[Serializable] // Для Binary и SOAP
[XmlInclude(typeof(ApplicationSoftware))] // Для полиморфной XML-сериализации
[XmlInclude(typeof(WordProcessor))]
[XmlInclude(typeof(Game))]
[XmlInclude(typeof(Sapper))]
[XmlInclude(typeof(Virus))]
[XmlInclude(typeof(CConficker))]
public class Software
{
    public string Version { get; set; }
    public Developer Developer { get; set; }

    // Пустой конструктор обязателен для сериализации
    public Software()
    {
        Version = "1.0";
        Developer = new Developer();
    }

    public Software(string v, Developer d)
    {
        Version = v;
        Developer = d;
    }

    public virtual void Run()
    {
        Console.WriteLine($"ПО (Версия: {Version}, Автор: {Developer.DeveloperName}) запущено.");
    }

    public void CheckForUpdates()
    {
        Console.WriteLine($"Проверка обновлений для версии {Version}...");
    }

    public override string ToString() => $"[Software: Версия={Version}, Разработчик={Developer.DeveloperName}]";
}

[Serializable]
public class ApplicationSoftware : Software
{
    // --- Поле, которое должно быть исключено из сериализации ---
    [NonSerialized] // Игнорировать в Binary/SOAP
    [XmlIgnore] // Игнорировать в XML
    [JsonIgnore] // Игнорировать в JSON
    private int _spentTime;

    // Публичное свойство нужно для установки значения в коде. 
    // При десериализации оно вернет 0 (значение по умолчанию) или null.
    public int SpentTime
    {
        get => _spentTime;
        set
        {
            if (value >= 0) _spentTime = value;
        }
    }
    public bool IsLicensed { get; set; }

    public ApplicationSoftware()
    {
        _spentTime = 0;
        IsLicensed = true;
    }
}

[Serializable]
public class Game : ApplicationSoftware
{
    public int CurrentLevel { get; set; }

    public Game()
    {
        CurrentLevel = 1;
    }
}

[Serializable]
public sealed class Sapper : Game
{
    public int MineCount { get; set; }

    public Sapper()
    {
        MineCount = 10;
        CurrentLevel = 1;
    }
}

[Serializable]
public class WordProcessor : ApplicationSoftware
{
    // Operations должен быть типом OperationSet, а не IOperationSet, 
    // чтобы его можно было сериализовать
    public OperationSet Operations { get; set; }

    public string CurrentDocumentPath { get; set; }

    public WordProcessor()
    {
        // Инициализация Operations как сериализуемый класс
        Operations = new OperationSet();
        Operations.AddOperation("Открыть файл");
        Operations.AddOperation("Сохранить документ");
        CurrentDocumentPath = "НовыйДокумент.docx";
    }

    public string GetStatusDescriprion() => Operations.GetStatus();

    public override string ToString() => $"[WordProcessor: (База: {base.ToString()}), Документ={CurrentDocumentPath}]";
}


[Serializable]
public abstract class MaliciousSoftware : Software
{
    // MaliciousSoftware использует композицию
    public bool IsDetected { get; private set; }

    public MaliciousSoftware()
    {
        IsDetected = false;
        Version = "1.0";
        // Изменение для сериализации: Developer должен быть конкретным объектом
        Developer = new Developer("Хакер", "Алексей");
    }

    public abstract void ExecutePayload();
}

[Serializable]
public class Virus : MaliciousSoftware
{
    public Virus() { } // Пустой конструктор для сериализации

    public override void ExecutePayload()
    {
        Console.WriteLine("Вирус: Начинается заражение файлов.");
    }
}

[Serializable]
public sealed class CConficker : Virus
{
    public CConficker()
    {
        Version = "A-2008";
    }
}

#endregion