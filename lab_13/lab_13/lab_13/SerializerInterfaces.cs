using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Text.Json;
using System.Text;

// Общий интерфейс для всех сериализаторов (Strategy Pattern)
public interface ISerializer
{
    void Serialize<T>(string filePath, T obj);
    T Deserialize<T>(string filePath);
}

#region Binary Serializer (Оставлен для выполнения задания, несмотря на устаревание)
// Отключаем предупреждение об устаревании BinaryFormatter
#pragma warning disable SYSLIB0011 
public class BinarySerializer : ISerializer
{
    public void Serialize<T>(string filePath, T obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
                bf.Serialize(fs, obj);
            Console.WriteLine($"[Binary] Сериализация в {filePath} успешна.");
        }
        catch (Exception ex) { Console.WriteLine($"Ошибка Binary-сериализации: {ex.Message}"); }
    }
    public T Deserialize<T>(string filePath)
    {
        BinaryFormatter bf = new BinaryFormatter();
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
                return (T)bf.Deserialize(fs);
        }
        catch (Exception ex) { Console.WriteLine($"Ошибка Binary-десериализации: {ex.Message}"); return default; }
    }
}
#pragma warning restore SYSLIB0011
#endregion


// JSON Serializer
public class JsonSerializerImpl : ISerializer
{
    public void Serialize<T>(string filePath, T obj)
    {
        try
        {
            string json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
            Console.WriteLine($"[JSON] Сериализация в {filePath} успешна.");
        }
        catch (Exception ex) { Console.WriteLine($"Ошибка JSON-сериализации: {ex.Message}"); }
    }
    public T Deserialize<T>(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex) { Console.WriteLine($"Ошибка JSON-десериализации: {ex.Message}"); return default; }
    }
}

// XML Serializer
public class XmlSerializerImpl : ISerializer
{
    public void Serialize<T>(string filePath, T obj)
    {
        XmlSerializer xs = new XmlSerializer(typeof(T));
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
                xs.Serialize(fs, obj);
            Console.WriteLine($"[XML] Сериализация в {filePath} успешна.");
        }
        catch (Exception ex) { Console.WriteLine($"Ошибка XML-сериализации: {ex.Message}"); }
    }
    public T Deserialize<T>(string filePath)
    {
        XmlSerializer xs = new XmlSerializer(typeof(T));
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
                return (T)xs.Deserialize(fs);
        }
        catch (Exception ex) { Console.WriteLine($"Ошибка XML-десериализации: {ex.Message}"); return default; }
    }
}