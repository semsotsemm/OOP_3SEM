using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // 1. Инициализация объекта и коллекции
        Sapper mySapper = new Sapper
        {
            Version = "Pro-7.0",
            MineCount = 50,
            SpentTime = 999, // Это поле будет проигнорировано при сериализации
            IsLicensed = true,
            Developer = new Developer("Ivan", "DevApps")
        };

        List<Sapper> sapperList = new List<Sapper> { mySapper, new Sapper { Version = "Lite-3.0", MineCount = 10, Developer = new Developer("Anon", "FreeSoft") } };

        Console.WriteLine($"\n*** Исходный объект: {mySapper.Version}, SpentTime: {mySapper.SpentTime} ***");

        // --- Задание 1: Демонстрация всех сериализаторов ---

        ISerializer[] serializers = new ISerializer[]
        {
            new BinarySerializer(),
            new JsonSerializerImpl(),
            new XmlSerializerImpl()
        };

        string[] extensions = { "bin", "json", "xml" }; // Удалена "soap"

        for (int i = 0; i < serializers.Length; i++)
        {
            string path = $"sapper_obj.{extensions[i]}";

            // Сериализация (сохранение)
            CustomSerializer.Save(serializers[i], path, mySapper);

            // Десериализация (загрузка)
            Sapper loadedSapper = CustomSerializer.Load<Sapper>(serializers[i], path);

            if (loadedSapper != null)
            {
                Console.WriteLine($"\nДесериализовано: Версия: {loadedSapper.Version}, Мины: {loadedSapper.MineCount}");
                Console.WriteLine($"Проверка игнорирования: SpentTime = {loadedSapper.SpentTime} (Должно быть 0).");
            }
        }

        // --- Задание 2: Сериализация коллекции ---

        string listPath = "sapper_list.json";
        CustomSerializer.Save(new JsonSerializerImpl(), listPath, sapperList);
        List<Sapper> loadedList = CustomSerializer.Load<List<Sapper>>(new JsonSerializerImpl(), listPath);
        Console.WriteLine($"\nКоллекция успешно загружена. Элементов: {loadedList.Count}");

        // --- Задание 3: XPath и LINQ to XML ---

        // Для этого нужно, чтобы XML-файл уже существовал
        CustomSerializer.Save(new XmlSerializerImpl(), "sapper_obj.xml", mySapper);
        XmlProcessor.RunQueries("sapper_obj.xml");

        // --- Усложненное задание: Сокеты ---

        Console.WriteLine("\n--- Усложненное задание: Сокеты ---");

        // Запускаем сервер в отдельном потоке, чтобы он не блокировал Main
        Task.Run(() => SocketTransport.StartServer());
        Thread.Sleep(500); // Даем время серверу запуститься

        SocketTransport.StartClient(mySapper);

        Console.WriteLine("\nДемонстрация завершена. Проверьте созданные файлы.");
        Console.ReadKey();
    }
}