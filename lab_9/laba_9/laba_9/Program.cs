using System.Collections.ObjectModel;
using System.Collections.Specialized;


public class Car : IComparable<Car>
{
    public int Id { get; set; }
    public string Brand { get; set; } 
    public int Year { get; set; }     

    public Car(int id, string brand, int year)
    {
        Id = id;
        Brand = brand;
        Year = year;
    }

    public override string ToString()
    {
        return $"[ID: {Id}, Марка: {Brand}, Год: {Year}]";
    }

    public int CompareTo(Car other)
    {
        if (other == null) return 1;
        return this.Id.CompareTo(other.Id);
    }
}

public interface ICarList : IList<Car>
{
    bool AddCar(int id, Car car);
    Car FindCarById(int id);
}

public class CarCollectionManager : ICarList
{
    private readonly Dictionary<int, Car> _cars = new Dictionary<int, Car>();

    public int Count => _cars.Count;
    public bool IsReadOnly => false;

    public bool AddCar(int id, Car car)
    {
        if (_cars.ContainsKey(id)) return false;
        _cars.Add(id, car);
        return true;
    }

    public Car FindCarById(int id)
    {
        _cars.TryGetValue(id, out Car car);
        return car;
    }

    public void DisplayAll()
    {
        foreach (var pair in _cars)
        {
            Console.WriteLine($"Ключ: {pair.Key}, Значение: {pair.Value}");
        }
    }

    public void Add(Car item)
    {
        int newId = _cars.Any() ? _cars.Keys.Max() + 1 : 1000;
        item.Id = newId;
        _cars.Add(newId, item);
    }

    public bool Remove(Car item)
    {
        var key = _cars.FirstOrDefault(x => x.Value.Id == item.Id).Key;
        if (key != default(int))
        {
            return _cars.Remove(key);
        }
        return false;
    }

    public bool Remove(int id)
    {
        return _cars.Remove(id);
    }

    public Car this[int index]
    {
        get => _cars.ElementAt(index).Value;
        set => throw new NotSupportedException("Операция по индексу не поддерживается для Dictionary.");
    }
    public void Insert(int index, Car item) => throw new NotSupportedException("Операция Insert по индексу не поддерживается для Dictionary.");
    public void RemoveAt(int index) => _cars.Remove(_cars.ElementAt(index).Key);
    public bool Contains(Car item) => _cars.Values.Contains(item);
    public void Clear() => _cars.Clear();
    public int IndexOf(Car item) => throw new NotSupportedException("Операция IndexOf не поддерживается для Dictionary.");
    public void CopyTo(Car[] array, int arrayIndex) => _cars.Values.CopyTo(array, arrayIndex);
    public IEnumerator<Car> GetEnumerator() => _cars.Values.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}


public class CarDemo
{
    public static void Run()
    {
        Console.WriteLine("1. Демонстрация работы с классом Автомобиль (Dictionary<int, Car>)");

        var manager = new CarCollectionManager();

        manager.AddCar(101, new Car(101, "Lada", 2010));
        manager.AddCar(102, new Car(102, "Toyota", 2015));
        manager.AddCar(103, new Car(103, "BMW", 2020));
        manager.Add(new Car(0, "Audi", 2022));

        manager.DisplayAll();

        Console.WriteLine("\nПоиск элемента (ID: 102)");
        var found = manager.FindCarById(102);
        Console.WriteLine(found != null ? $"Найдено: {found}" : "Не найдено.");

        Console.WriteLine("\nУдаление элемента (ID: 101)");
        if (manager.Remove(101))
        {
            Console.WriteLine("Автомобиль с ID 101 удален.");
        }

        Console.WriteLine("\nКоллекция после удаления");
        manager.DisplayAll();
    }
}


public class NetCollectionDemo
{
    public static void Run()
    {
        Console.WriteLine("\n\n1.3. Работа с коллекциями встроенных типов (int)");

        Dictionary<int, int> dict1 = new Dictionary<int, int>
        {
            { 1, 100 }, { 2, 200 }, { 3, 300 }, { 4, 400 }, { 5, 500 }, { 6, 600 }
        };

        Console.WriteLine("\n1.a. Исходная коллекция dict1 (Dictionary<int, int>)");
        PrintDictionary(dict1);

        int n = 3;
        Console.WriteLine($"\n1.b. Удаление {n} элементов (ключи 2, 3, 4)");
        dict1.Remove(2);
        dict1.Remove(3);
        dict1.Remove(4);
        PrintDictionary(dict1);

        Console.WriteLine("\n1.c. Добавление других элементов");
        dict1.Add(7, 777);
        dict1.TryAdd(8, 888);
        PrintDictionary(dict1);

        Console.WriteLine("\n1.d. Создание и заполнение List<int> из dict1.Values");
        List<int> list2 = new List<int>(dict1.Values);

        Console.WriteLine("\n1.e. Вторая коллекция list2 (List<int>)");
        Console.WriteLine($"[{string.Join(", ", list2)}]");

        int searchValue = 777;
        Console.WriteLine($"\n1.f. Поиск заданного значения ({searchValue}) в list2");
        int index = list2.IndexOf(searchValue);
        Console.WriteLine(index != -1 ? $"Значение {searchValue} найдено по индексу: {index}." : $"Значение {searchValue} не найдено.");
    }

    private static void PrintDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict)
    {
        foreach (var pair in dict)
        {
            Console.WriteLine($"Ключ: {pair.Key}, Значение: {pair.Value}");
        }
    }
}


public class ObservableDemo
{
    public static void Run()
    {
        Console.WriteLine("\n\n2. ObservableCollection<Car>");

        ObservableCollection<Car> carsObserved = new ObservableCollection<Car>();

        carsObserved.CollectionChanged += OnCarsCollectionChanged;

        Console.WriteLine("\nДемонстрация с добавлением и удалением");

        Console.WriteLine("\nДобавление");
        carsObserved.Add(new Car(201, "Mercedes", 2021));
        carsObserved.Add(new Car(202, "Mazda", 2018));

        Console.WriteLine("\nУдаление");
        if (carsObserved.Count > 0)
        {
            carsObserved.RemoveAt(0);
        }

        Console.WriteLine("\nОчистка");
        carsObserved.Clear();

        carsObserved.CollectionChanged -= OnCarsCollectionChanged;
    }

    private static void OnCarsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n\tСобытие CollectionChanged (Обработчик)");
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                Console.WriteLine($"\tДобавлен: {((Car)e.NewItems[0]).Brand}");
                break;
            case NotifyCollectionChangedAction.Remove:
                Console.WriteLine($"\tУдален: {((Car)e.OldItems[0]).Brand}");
                break;
            case NotifyCollectionChangedAction.Reset:
                Console.WriteLine("\tОчистка коллекции.");
                break;
            default:
                Console.WriteLine($"\tДействие: {e.Action}");
                break;
        }
        Console.ResetColor();
    }
}


public class Program
{
    public static void Main(string[] args)
    {
        CarDemo.Run();
        Console.WriteLine("\n----------------------------------------------");
        NetCollectionDemo.Run();
        Console.WriteLine("\n----------------------------------------------");
        ObservableDemo.Run();
    }
}