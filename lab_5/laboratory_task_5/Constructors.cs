public partial class Software
{
    public Software()
    {
        _version = "11.7";
        _developer = new Developer();
    }

}

public partial class ApplicationSoftware : Software
{
    public ApplicationSoftware()
    {
        _spentTime = 0;
        License = LicenseType.Free;
    }
}

public partial class WordProcessor : ApplicationSoftware
{
    public WordProcessor()
    {
        Operations = new OperationSet();
        Operations.AddOperation("Открыть файл");
        Operations.AddOperation("Сохранить документ");
        Operations.AddOperation("Форматировать текст");
        Console.WriteLine("Текстовый процессор инициализирован с набором операций.");
        _currentDocumentPath = "НовыйДокумент.docx";
    }
}

public partial class Game : ApplicationSoftware
{
    public Game()
    {
        _currentLevel = 1;
    }
}
