using System.Text.Json; 
using System.Text.Json.Serialization; 

namespace Lab7_Generics
{
    public interface IGeneralOperations<T>
    {
        void Add(T item);
        void Remove(T item);
        void View();
    }

    public class CollectionType<T> : IGeneralOperations<T> where T : new()
    {
        public List<T> _items;

        public CollectionType()
        {
            _items = new List<T>();
        }

        public void Add(T item)
        {
            try
            {
                if (item == null)
                    throw new ArgumentNullException(nameof(item), "Нельзя добавить пустой объект (null).");

                _items.Add(item);
                Console.WriteLine($"[Успех] Добавлен элемент: {item}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка добавления] {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Операция добавления завершена."); 
            }
        }
        public void Remove(T item)
        {
            try
            {
                if (_items.Contains(item))
                {
                    _items.Remove(item);
                    Console.WriteLine($"[Успех] Элемент удален: {item}");
                }
                else
                {
                    Console.WriteLine($"[Ошибка] Элемент не найден в коллекции.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка удаления] {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Блок finally: Завершение операции удаления.");
            }
        }

        public void View()
        {
            Console.WriteLine("\n--- Содержимое коллекции ---");
            if (_items.Count == 0)
            {
                Console.WriteLine("Коллекция пуста.");
            }
            else
            {
                foreach (var item in _items)
                {
                    Console.WriteLine(item.ToString());
                }
            }
            Console.WriteLine("----------------------------\n");
        }

        public T Find(Predicate<T> match)
        {
            Console.WriteLine("Поиск элемента...");
            return _items.Find(match);
        }

        public void SaveToFile(string filePath)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true 
                };

                string jsonString = JsonSerializer.Serialize(_items, options);
                File.WriteAllText(filePath, jsonString);
                Console.WriteLine($"[Файл] Коллекция сохранена в {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка записи] {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Операция с файлом (Save) завершена.");
            }
        }

        public void LoadFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"[Ошибка] Файл {filePath} не существует.");
                    return;
                }

                string jsonString = File.ReadAllText(filePath);
                List<T> loadedItems = JsonSerializer.Deserialize<List<T>>(jsonString);

                if (loadedItems != null)
                {
                    _items = loadedItems;
                    Console.WriteLine($"[Файл] Коллекция загружена из {filePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Ошибка чтения] {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Операция с файлом (Load) завершена.");
            }
        }
    }


    public class Developer
    {
        public string Fio { get; set; }
        public string Department { get; set; }

        public Developer() 
        {
            Fio = "Неизвестно";
            Department = "IT";
        }

        public Developer(string fio, string dept)
        {
            Fio = fio;
            Department = dept;
        }

        public override string ToString() => $"{Fio} ({Department})";
    }

    [JsonDerivedType(typeof(Software), typeDiscriminator: "base")]
    [JsonDerivedType(typeof(Word), typeDiscriminator: "word")]
    [JsonDerivedType(typeof(Virus), typeDiscriminator: "virus")]
    public class Software
    {
        public string Version { get; set; }
        public Developer Author { get; set; }

        public Software()
        {
            Version = "1.0";
            Author = new Developer();
        }

        public override string ToString()
        {
            return $"[Software] Ver: {Version}, Dev: {Author}";
        }
    }

    public class Word : Software
    {
        public string CurrentDocument { get; set; }

        public Word()
        {
            Version = "Word 2024";
            CurrentDocument = "NewDoc.docx";
            Author = new Developer("Bill Gates", "Microsoft");
        }

        public override string ToString()
        {
            return $"[Word] Ver: {Version}, Doc: {CurrentDocument}, Dev: {Author.Fio}";
        }
    }

    public class Virus : Software
    {
        public string VirusType { get; set; }

        public Virus()
        {
            Version = "X-Trojan";
            VirusType = "Spyware";
            Author = new Developer("Hacker1337", "DarkNet");
        }

        public override string ToString()
        {
            return $"[Virus] Type: {VirusType}, Ver: {Version}, Dev: {Author.Fio}";
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Тест 1: Стандартный тип (int)");
            CollectionType<int> intCollection = new CollectionType<int>();

            intCollection.Add(10);
            intCollection.Add(255);
            intCollection.Add(-50);

            intCollection.View();

            Console.WriteLine("Удаляем 255...");
            intCollection.Remove(255);

            Console.WriteLine("Пытаемся удалить 999 (нет в списке)...");
            intCollection.Remove(999);

            intCollection.View();

            intCollection.SaveToFile("numbers.json");

            Console.WriteLine("\nтест 2: Пользовательский тип (Software)");
            CollectionType<Software> softCollection = new CollectionType<Software>();

            Word myWord = new Word { CurrentDocument = "Lab7_Report.docx" };
            Virus myVirus = new Virus { VirusType = "Ransomware" };
            Software baseSoft = new Software { Version = "0.1 Beta" };

            softCollection.Add(myWord);
            softCollection.Add(myVirus);
            softCollection.Add(baseSoft);

            softCollection.View();

            Console.WriteLine("Поиск ПО, где версия 'X-Trojan':");
            var foundItem = softCollection.Find(s => s.Version == "X-Trojan");
            if (foundItem != null)
                Console.WriteLine($"Найдено: {foundItem}");
            else
                Console.WriteLine("Не найдено.");

            Console.WriteLine("\nтест 3: Файловые операции (JSON)");
            string filename = "software_data.json";

            softCollection.SaveToFile(filename);

            Console.WriteLine("\n(Очистка памяти для теста загрузки...)");
            softCollection = new CollectionType<Software>();
            softCollection.View(); 

            Console.WriteLine("Загрузка данных из файла...");
            softCollection.LoadFromFile(filename);

            softCollection.View();
        }
    }
}