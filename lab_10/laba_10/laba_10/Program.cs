using System;
using System.Collections.Generic;
using System.Linq;

public class Car
{
    public int Id { get; set; }
    public string Brand { get; set; }     
    public int Year { get; set; }         
    public string Color { get; set; }     
    public double EngineVolume { get; set; } 

    public override string ToString()
    {
        return $"[ID: {Id}, Марка: {Brand}, Год: {Year}, Объем: {EngineVolume}L]";
    }
}

public class Owner
{
    public int CarId { get; set; }
    public string FullName { get; set; }
}

public class Vector
{
    public double[] Elements { get; set; }
    public int Length => Elements.Length;

    public double Magnitude() => Math.Sqrt(Elements.Sum(e => e * e));

    public override string ToString()
    {
        return $"Длина {Length}, Модуль: {Magnitude():F2}, Элементы: [{string.Join(", ", Elements)}]";
    }
}


public class Program
{
    public static void Main(string[] args)
    {
        DemoStringArrayLinq();
        Console.WriteLine("\n" + new string('-', 70));
        DemoCarCollectionLinq();
        Console.WriteLine("\n" + new string('-', 70));
        DemoComplexLinq();
        Console.WriteLine("\n" + new string('-', 70));
        DemoVectorLinq();
    }


    private static void DemoStringArrayLinq()
    {
        Console.WriteLine("1. LINQ к массиву строк (Месяцы)");

        string[] months = new string[]
        {
            "January", "February", "March", "April",
            "May", "June", "July", "August",
            "September", "October", "November", "December"
        };

        int n = 5;
        var lengthNMonths = from month in months
                            where month.Length == n
                            select month;
        Console.WriteLine($"\n[1.1] Месяцы длиной {n}: {string.Join(", ", lengthNMonths)}");

        var summerWinterMonths = months.Where(m =>
            (m == "June" || m == "July" || m == "August") ||
            (m == "December" || m == "January" || m == "February")
        );
        Console.WriteLine($"\n[1.2] Летние и зимние: {string.Join(", ", summerWinterMonths)}");

        var alphabeticalMonths = from month in months
                                 orderby month ascending
                                 select month;
        Console.WriteLine($"\n[1.3] В алфавитном порядке: {string.Join(", ", alphabeticalMonths.Take(6))}...");

        int countUMonths = months
            .Where(m => m.Contains("u") && m.Length >= 4)
            .Count();
        Console.WriteLine($"\n[1.4] Количество месяцев с 'u' и длиной >= 4: {countUMonths}");
    }


    private static void DemoCarCollectionLinq()
    {
        Console.WriteLine("2. и 3. LINQ к коллекции List<Car");

        List<Car> cars = new List<Car>
        {
            new Car { Id = 1, Brand = "Toyota", Year = 2020, Color = "Red", EngineVolume = 2.0 },
            new Car { Id = 2, Brand = "BMW", Year = 2018, Color = "Black", EngineVolume = 3.0 },
            new Car { Id = 3, Brand = "Lada", Year = 2022, Color = "White", EngineVolume = 1.6 },
            new Car { Id = 4, Brand = "Toyota", Year = 2021, Color = "Gray", EngineVolume = 2.5 },
            new Car { Id = 5, Brand = "Audi", Year = 2019, Color = "Blue", EngineVolume = 2.0 },
            new Car { Id = 6, Brand = "Mercedes", Year = 2023, Color = "Black", EngineVolume = 4.0 },
            new Car { Id = 7, Brand = "Lada", Year = 2020, Color = "Red", EngineVolume = 1.6 },
            new Car { Id = 8, Brand = "BMW", Year = 2023, Color = "White", EngineVolume = 3.0 },
            new Car { Id = 9, Brand = "Audi", Year = 2018, Color = "Black", EngineVolume = 1.8 },
            new Car { Id = 10, Brand = "Toyota", Year = 2023, Color = "Red", EngineVolume = 2.0 }
        };

        Console.WriteLine("\n[2] Запрос красные, после 2019 года (Метод расширения):");
        var filteredCars = cars
            .Where(car => car.Color == "Red" && car.Year > 2019) 
            .Select(car => new
            {
                car.Brand,
                car.Year
            });

        foreach (var car in filteredCars)
        {
            Console.WriteLine($"  -> {car.Brand} ({car.Year})");
        }

        List<Owner> owners = new List<Owner>
        {
            new Owner { CarId = 1, FullName = "Иванов И.И." },
            new Owner { CarId = 3, FullName = "Петров П.П." },
            new Owner { CarId = 10, FullName = "Сидоров С.С." },
            new Owner { CarId = 5, FullName = "Алексеев А.А." }
        };

        Console.WriteLine("\n[3] Запрос с оператором Join (Машины с владельцами):");
        var joinQuery = from car in cars
                        join owner in owners on car.Id equals owner.CarId
                        select new
                        {
                            car.Brand,
                            owner.FullName
                        };

        foreach (var item in joinQuery)
        {
            Console.WriteLine($"  -> Машина: {item.Brand}, Владелец: {item.FullName}");
        }
    }


    private static void DemoComplexLinq()
    {
        Console.WriteLine("4. Собственный сложный запрос (5 операторов)");

        List<Car> cars = new List<Car>
        {
            new Car { Id = 1, Brand = "Toyota", Year = 2020, Color = "Red", EngineVolume = 2.0 },
            new Car { Id = 2, Brand = "BMW", Year = 2018, Color = "Black", EngineVolume = 3.0 },
            new Car { Id = 3, Brand = "Lada", Year = 2022, Color = "White", EngineVolume = 1.6 },
            new Car { Id = 4, Brand = "Toyota", Year = 2021, Color = "Gray", EngineVolume = 2.5 },
            new Car { Id = 5, Brand = "Audi", Year = 2019, Color = "Blue", EngineVolume = 2.0 },
            new Car { Id = 6, Brand = "Mercedes", Year = 2023, Color = "Black", EngineVolume = 4.0 },
            new Car { Id = 7, Brand = "Lada", Year = 2020, Color = "Red", EngineVolume = 1.6 },
            new Car { Id = 8, Brand = "BMW", Year = 2023, Color = "White", EngineVolume = 3.0 },
            new Car { Id = 9, Brand = "Audi", Year = 2018, Color = "Black", EngineVolume = 1.8 },
            new Car { Id = 10, Brand = "Toyota", Year = 2023, Color = "Red", EngineVolume = 2.0 }
        };

        var complexQuery = cars
            .GroupBy(car => car.Brand)                                  
            .OrderByDescending(group => group.Count())                  
            .Where(group => group.Count() > 1)                          
            .Select(group => group.OrderByDescending(car => car.Year).First()) 
            .Take(2)                                                    
            .ToList();

        Console.WriteLine("\n[4] Результат сложного запроса (Самые новые машины из двух самых популярных марок):");
        foreach (var car in complexQuery)
        {
            Console.WriteLine($"  -> Марка: {car.Brand}, Год: {car.Year}");
        }
    }


    private static void DemoVectorLinq()
    {
        Console.WriteLine("5. Запросы к массиву векторов");

        Vector[] vectors = new Vector[]
        {
            new Vector { Elements = new double[] { 1, 0, 3 } },         
            new Vector { Elements = new double[] { -2, 5, -1, 4, 0 } }, 
            new Vector { Elements = new double[] { 0, 1, 0, 0, 0, 2, 8 } }, 
            new Vector { Elements = new double[] { 10, 20, 30 } },    
            new Vector { Elements = new double[] { 4, 2, 6, 1, 5 } }, 
            new Vector { Elements = new double[] { 1, 1, 1, 1, 1, 1, 1 } } 
        };

        int countZeroVectors = vectors.Count(v => v.Elements.Contains(0.0));
        Console.WriteLine($"\n[5.1] Количество векторов, содержащих 0: {countZeroVectors}");

        double minMagnitude = vectors.Min(v => v.Magnitude());
        var minMagnitudeVectors = vectors.Where(v => v.Magnitude() == minMagnitude);
        Console.WriteLine($"\n[5.2] Векторы с наименьшим модулем ({minMagnitude:F2}):");
        foreach (var v in minMagnitudeVectors)
        {
            Console.WriteLine($"  -> {v}");
        }

        var maxVector = vectors.OrderByDescending(v => v.Magnitude()).First();
        Console.WriteLine($"\n[5.3] Максимальный вектор (по модулю): {maxVector}");

        var firstNegativeVector = vectors.FirstOrDefault(v => v.Elements.Any(e => e < 0));
        Console.WriteLine($"\n[5.4] Первый вектор с отрицательным значением: {firstNegativeVector}");

        var sortedByLength = from v in vectors
                             orderby v.Length ascending
                             select v;
        Console.WriteLine("\n[5.5] Векторы, отсортированные по длине:");
        foreach (var v in sortedByLength)
        {
            Console.WriteLine($"  -> {v.Length}");
        }
    }
}