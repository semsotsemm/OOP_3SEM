public partial class ApplicationSoftware : Software
{
    public override void Run()
    {
        Console.WriteLine($"ПО запущено. Лицензия: {(License == LicenseType.Free ? "Активна" : "Отсутствует")}.");
    }
    public void Activate(string key)
    {
        if (!string.IsNullOrEmpty(key) && key.Length > 5)
        {
            License = LicenseType.Free;
            Console.WriteLine("Приложение успешно активировано.");
        }
        else
        {
            Console.WriteLine("Неверный ключ активации.");
        }
    }

    public override string ToString()
    {
        return $"[ApplicationSoftware: (База: {base.ToString()}), Время={SpentTime}, Лицензия={License}]";
    }
}
