using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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

public partial class Software
{
    private string _version;
    private Developer _developer;
}

public partial class ApplicationSoftware : Software
{
    private int _spentTime;
    public enum LicenseType
    {
        Free,
        Trial,
        Paid,
        Cracked
    }

    private LicenseType _licenseType;
}

public partial class WordProcessor : ApplicationSoftware
{
    private readonly IOperationSet Operations;
    private string _currentDocumentPath;
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

public partial class Game : ApplicationSoftware
{
    private int _currentLevel;
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

public class Computer 
{
    private List<Software> _softwares;
    private int hardDrive;

    public List<Software> Softwares 
    {
        get 
        {
       return _softwares;
        }
        set 
        {
            if (value == null || value.Count == 0)
            {
                Console.WriteLine("Список программных обеспечений не может быть пустым");
                return;
            }
            else 
            {
                _softwares = value;
            }
        }

    }

    public Computer() 
    {
        hardDrive = 1024;
        _softwares = new List<Software>();
    }

    public bool Install(Software Software)
    {
        bool IsInstalled = _softwares.Contains(Software);
        if (!IsInstalled)
        {
            _softwares.Add(Software);
            Console.WriteLine("Программа устанавливается...");
        }
        return !IsInstalled;
    }
    public bool Uninstall(Software Software)
    {
        bool IsInstalled = _softwares.Contains(Software);
        if (IsInstalled)
        {
            _softwares.Remove(Software);
            Console.WriteLine("Удаление программы...");
        }
        return IsInstalled;
    }
    public void ShowInstalledSoftware()
    {
        Console.WriteLine("Список установленных программ:");
        for (int i = 0; i < _softwares.Count; i++)
        {
            Console.Write($"{i+1}: {_softwares[i]}");
            if (i < _softwares.Count - 1)
            {
                Console.Write("\n");
            }
        }
    }
}

public class ComputerController 
{
    private readonly Computer _computer;

    public ComputerController(Computer computer)
    {
        _computer = computer;
    }

    public void LoadFromTextFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Текстовый файл не найден!");
            return;
        }

        foreach (var line in File.ReadLines(filePath))
        {
            Software software = CreateSoftwareByName(line.Trim());
            if (software != null)
            {
                _computer.Install(software);
            }
        }
    }

    public void LoadFromJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("JSON файл не найден!");
            return;
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
    public List<string> FindGames()
    {
        List<string> ListOfGames = new List<string>();

        for (int i = 0; i < _computer.Softwares.Count; i++)
        {
            if (_computer.Softwares[i] is Game game)
            {
                ListOfGames.Add(game.ToString());
            }
        }

        return ListOfGames;
    }
    public Word FindTextEditor(string version)
    {
        Word TextEditor = null;

        for (int i = 0; i < _computer.Softwares.Count; i++)
        {
            if (_computer.Softwares[i] is Word word)
            {
                if (word.Version == version)
                {
                    TextEditor = word;
                    break;
                }
            }
        }

        return TextEditor;
    }
    public void printInAlphabeticalOrder()
    {
        if (_computer.Softwares.Count == 0)
        {
            Console.WriteLine("Нет установленных программ.");
            return;
        }

        Console.WriteLine("Программы в алфавитном порядке:");

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
    static void Print(string text, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Debug.Assert(text == null, "НУ ТЫ И ЕМЕЛЯ");
        Console.WriteLine(text);
        Console.ResetColor();
    }

    static void PrepareFiles()
    {
        File.WriteAllLines("software.txt", new string[]
        {
            "Word",
            "Sapper",
            "CConficker"
        });

        File.WriteAllText("software.json",
            JsonConvert.SerializeObject(
                new List<string> { "Sapper", "Word" },
                Formatting.Indented
            )
        );

        Print("Файлы software.txt и software.json подготовлены ✅", ConsoleColor.Green);
    }

    static void Main(string[] args)
    {
        Print("===== СТАРТ ТЕСТИРОВАНИЯ 📌 =====", ConsoleColor.Cyan);

        PrepareFiles();

        Computer computer = new Computer();
        ComputerController controller = new ComputerController(computer);

        Print("\n=== ТЕСТ 1 — Загрузка из TXT ===", ConsoleColor.Yellow);
        controller.LoadFromTextFile("software.txt");

        Print("\n=== ТЕСТ 2 — Установка вручную ===", ConsoleColor.Yellow);
        Software game = new Sapper();
        Software virus = new CConficker();

        Print(computer.Install(game) ? "Игра установлена ✅" : "Игра уже установлена ❌",
            ConsoleColor.Magenta);
        Print(computer.Install(virus) ? "Вирус установлен ✅" : "Вирус уже установлен ❌",
            ConsoleColor.Red);

        Print("\n📌 Установленные программы:", ConsoleColor.DarkCyan);
        computer.ShowInstalledSoftware();

        Print("\n=== ТЕСТ 3 — Удаление вредоноса ===", ConsoleColor.Yellow);
        Print(computer.Uninstall(virus) ? "Вирус удалён ✅" : "Вирус не найден ❌",
            ConsoleColor.DarkRed);

        Print("\n📌 После удаления:", ConsoleColor.DarkCyan);
        computer.ShowInstalledSoftware();

        Print("\n=== ТЕСТ 4 — Загрузка из JSON ===", ConsoleColor.Yellow);
        controller.LoadFromJson("software.json");
        computer.ShowInstalledSoftware();

        Print("\n=== ТЕСТ 5 — Игры на ПК ===", ConsoleColor.Yellow);
        var games = controller.FindGames();
        foreach (var g in games)
            Print(g, ConsoleColor.Blue);

        Print("\n=== ТЕСТ 6 — Поиск Word по версии ===", ConsoleColor.Yellow);
        var foundWord = controller.FindTextEditor("11.7");
        Print(foundWord != null ? $"✅ Найден: {foundWord}" : "❌ Не найден", ConsoleColor.Green);

        Print("\n=== ТЕСТ 7 — Сортировка ПО ===", ConsoleColor.Yellow);
        controller.printInAlphabeticalOrder();

        Print("\n===== ТЕСТЫ УСПЕШНО ЗАВЕРШЕНЫ ✅ =====", ConsoleColor.Cyan);
    }
}
