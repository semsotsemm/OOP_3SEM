using System.Diagnostics;

public partial class Software
{
    public Developer Developer
    {
        get { return _developer; }
        set
        {
            if (string.IsNullOrEmpty(value.DeveloperName) || string.IsNullOrEmpty(value.CompanyName))
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
}

public partial class ApplicationSoftware : Software
{
    public LicenseType License
    {
        get { return _licenseType; }
        set { _licenseType = value; }
    }
    public int SpentTime 
    {
        get 
        {
            return _spentTime;
        }
        set 
        {
            if (value > 0) 
            {
                _spentTime = value;
            }
        }
    }

}

public partial class WordProcessor : ApplicationSoftware
{
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
}

public partial class Game : ApplicationSoftware
{
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
}