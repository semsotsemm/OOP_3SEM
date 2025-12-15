using System;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;

// 1. CustomSerializer для унифицированной работы
public static class CustomSerializer
{
    public static void Save<T>(ISerializer serializer, string path, T obj)
    {
        Console.WriteLine($"\n--- Запуск сохранения (Тип: {serializer.GetType().Name}) ---");
        serializer.Serialize(path, obj);
    }

    public static T Load<T>(ISerializer serializer, string path)
    {
        Console.WriteLine($"\n--- Запуск загрузки (Тип: {serializer.GetType().Name}) ---");
        return serializer.Deserialize<T>(path);
    }
}

// 2. XPath и LINQ to XML
public class XmlProcessor
{
    public static void RunQueries(string xmlPath)
    {
        try
        {
            Console.WriteLine("\n--- 3. Работа с XML-документом (XPath и LINQ to XML) ---");
            XDocument doc = XDocument.Load(xmlPath);

            // XPath 1: Выбрать имя разработчика Sapper
            var devName = doc.XPathSelectElement("//Sapper/Developer/DeveloperName");
            Console.WriteLine($"XPath 1: Имя разработчика: {devName?.Value ?? "N/A"}");

            // XPath 2: Выбрать версию (Version) и количество мин (MineCount)
            var sapperInfo = doc.XPathSelectElement("//Sapper");
            Console.WriteLine($"XPath 2: Версия: {sapperInfo?.Element("Version")?.Value}, Мины: {sapperInfo?.Element("MineCount")?.Value}");

            // LINQ to XML: Создание нового документа
            XDocument newDoc = new XDocument(
                new XElement("GameSummary",
                    doc.Descendants("Sapper").Select(s =>
                        new XElement("SapperGame",
                            new XAttribute("Version", s.Element("Version").Value),
                            new XElement("Mines", s.Element("MineCount").Value)
                        )
                    )
                )
            );
            string newXmlPath = "new_sapper_summary.xml";
            newDoc.Save(newXmlPath);
            Console.WriteLine($"LINQ to XML: Создан новый документ {newXmlPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в XmlProcessor: {ex.Message}");
        }
    }
}

// 2. Усложненное задание: Клиент-Сервер (Сокеты)
public class SocketTransport
{
    private const int Port = 8888;

    public static void StartServer()
    {
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, Port);
        using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            socket.Bind(ipPoint);
            socket.Listen(1);
            Console.WriteLine("Сервер: Запущен. Ожидание подключения...");

            using Socket client = socket.Accept();
            byte[] buffer = new byte[2048];
            int bytes = client.Receive(buffer);
            string data = Encoding.UTF8.GetString(buffer, 0, bytes);

            // Десериализация на стороне сервера (для примера)
            Sapper receivedSapper = JsonSerializer.Deserialize<Sapper>(data);
            Console.WriteLine($"Сервер: Получен объект Sapper (Версия: {receivedSapper.Version}, Мины: {receivedSapper.MineCount})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сервера: {ex.Message}");
        }
    }

    public static void StartClient(Sapper obj)
    {
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Loopback, Port);
        using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            socket.Connect(ipPoint);

            // Сериализация в JSON для отправки
            string json = JsonSerializer.Serialize(obj);
            byte[] data = Encoding.UTF8.GetBytes(json);
            socket.Send(data);
            Console.WriteLine("Клиент: Данные Sapper отправлены.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка клиента: {ex.Message}. Убедитесь, что сервер запущен!");
        }
    }
}