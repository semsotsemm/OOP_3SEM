using System;
using System.Linq;

public partial class Vector
{
    public static int _objectCount = 0;
    private static int nextId = 1;
    private const int VersionInfo = 1;
    public static string Version;

    public static void PrintClassInfo()
    {
        Console.WriteLine("\nИнформация о классе Vector ");
        Console.WriteLine($"Версия класса: {Version}");
        Console.WriteLine($"Количество созданных объектов: {_objectCount}");
    }

    private int[] values;
    private int size;
    private int statusCode;
    private readonly int id;

    public int[] Values
    {
        get { return values; }
        private set
        {
            values = value;
            size = values?.Length ?? 0;
        }
    }

    public int Size
    {
        get { return size; }
        private set
        {
            if (value >= 0)
            {
                size = value;
                Array.Resize(ref values, value);
                this.StatusCode = 0;
            }
            else
            {
                this.StatusCode = -1;
            }
        }
    }

    public int StatusCode
    {
        get { return statusCode; }
        private set { statusCode = value; }
    }

    public int Id
    {
        get { return id; }
    }

    public double Magnitude
    {
        get
        {
            if (this.Size == 0) return 0.0;
            long sumOfSquares = this.Values.Sum(v => (long)v * v);
            return Math.Sqrt(sumOfSquares);
        }
    }

    static Vector()
    {
        Version = $"Vector Class v{VersionInfo}";
    }

    public Vector()
    {
        _objectCount++;
        id = HashVectorId(nextId++);
        this.Values = new int[0];
        this.StatusCode = 0;
    }

    public Vector(int size) : this()
    {
        if (size < 0)
        {
            this.StatusCode = -1;
        }
        else
        {
            this.Size = size;
        }
    }

    public Vector(int size, int[] initialValues) : this()
    {
        if (size < 0)
        {
            this.StatusCode = -1;
        }
        else if (initialValues.Length != size)
        {
            this.StatusCode = -2;
        }
        else
        {
            this.Values = (int[])initialValues.Clone();
            this.StatusCode = 0;
        }
    }

    private Vector(bool isPrivate) : this()
    {
        Console.WriteLine("Приватный конструктор вызван");
    }

    public Vector Add(int scalar)
    {
        if (this.Size == 0) return new Vector();

        int[] newValues = this.Values.Select(v => v + scalar).ToArray();
        return new Vector(this.Size, newValues);
    }
    
    public Vector Multiply(int scalar)
    {
        if (this.Size == 0) return new Vector();

        int[] newValues = this.Values.Select(v => v * scalar).ToArray();
        return new Vector(this.Size, newValues);
    }

    public bool TryModifyAndCalculate(int index, ref int element, out long sum)
    {
        sum = 0;
        if (index < 0 || index >= this.Size)
        {
            this.StatusCode = -3;
            return false;
        }

        this.Values[index] = element;
        element += 100; 

        sum = this.Values.Sum(v => (long)v);
        this.StatusCode = 0;
        return true;
    }

    public void AddNewElement(int newElement)
    {
        int newSize = this.Size + 1;
        this.Size = newSize;
        this.Values[newSize - 1] = newElement;
    }

    public void PrintVector()
    {
        if (this.Size == 0)
        {
            Console.WriteLine("Vector is empty.");
            return;
        }
        Console.Write(string.Join(" -> ", this.Values));
        Console.WriteLine(" -> null");
    }

    public Vector CallPrivateConstructor()
    {
        return new Vector(true);
    }

    private int HashVectorId(int vectorCount)
    {
        return vectorCount * 4284 % 10000;
    }
}

public partial class Vector
{
    public override string ToString()
    {
        string elements = this.Size == 0
            ? "[Empty]"
            : $"[{string.Join(", ", this.Values)}]";

        return $"Vector (ID: {this.Id}, Size: {this.Size}, |V|: {this.Magnitude:F2}) Elements: {elements} (Status: {this.StatusCode})";
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Vector otherVector))
        {
            return false;
        }

        if (this.Size != otherVector.Size)
        {
            return false;
        }

        return this.Values.SequenceEqual(otherVector.Values);
    }

    public override int GetHashCode()
    {
        int hash = this.Size.GetHashCode();

        if (this.Values != null)
        {
            foreach (int value in this.Values)
            {
                hash = (hash * 397) ^ value.GetHashCode();
            }
        }

        return hash;
    }
}


class Program
{
    static void Main()
    {
        Console.WriteLine("Тест класса Vector: ");

        Vector v1 = new Vector(3, new int[] { 1, 2, 3 });
        Vector v2 = new Vector(3, new int[] { 10, 20, 30 });
        Vector v_empty = new Vector();

        Console.WriteLine($"v1: {v1}");
        Console.WriteLine($"v2: {v2}");
        Console.WriteLine($"v3(Пустой вектор): {v_empty}");

        int scalar = 5;
        Vector v_sum = v1.Add(scalar);
        Vector v_prod = v2.Multiply(scalar);

        Console.WriteLine("\nТест операций:");
        Console.WriteLine($"v1 + {scalar}: {v_sum}");
        Console.WriteLine($"v2 * {scalar}: {v_prod}");

        int refValue = 500;
        v1.TryModifyAndCalculate(index: 1, ref refValue, out long totalSum);

        Console.WriteLine("\nТест ref и out:");
        Console.WriteLine($"v1 после изменения: {v1}");
        Console.WriteLine($"Новая сумма (out): {totalSum}");
        Console.WriteLine($"refValue в Main: {refValue} (изменено)");

        Vector v_copy = new Vector(3, new int[] { 1, 600, 3 }); 

        Console.WriteLine("\nТест сравнения и статической информации:");
        Console.WriteLine($"v1.Equals(v_copy): {v1.Equals(v_copy)} (Ожидается True)");
        Console.WriteLine($"v1.GetHashCode() == v_copy.GetHashCode(): {v1.GetHashCode() == v_copy.GetHashCode()}");

        Vector.PrintClassInfo(); 


        Vector[] vectorArray = new Vector[]
        {
            new Vector(3, new int[] { 1, 2, 3 }),
            new Vector(4, new int[] { 0, 5, 10, 0 }),
            new Vector(2, new int[] { 1, 0 }),
            new Vector(1, new int[] { 100 }),
            new Vector(3, new int[] { -1, -1, -1 })
        };

        Console.WriteLine("Задания с массивом:");

        Console.WriteLine("\n   a) Список векторов, содержащих 0:");
        var vectorsWithZero = vectorArray
            .Where(v => v.Values != null && v.Values.Contains(0))
            .ToList();

        foreach (var v in vectorsWithZero)
        {
            Console.WriteLine(v);
        }

        Console.WriteLine("\n   b) Список векторов с наименьшим модулем:");
        if (vectorArray.Length > 0)
        {
            double minMagnitude = vectorArray.Min(v => v.Magnitude);

            var smallestMagnitudeVectors = vectorArray
                .Where(v => Math.Abs(v.Magnitude - minMagnitude) < 0.0001)
                .ToList();

            foreach (var v in smallestMagnitudeVectors)
            {
                Console.WriteLine(v);
            }
            Console.WriteLine($"Наименьший модуль: {minMagnitude:F2}");
        }

        Console.WriteLine("\nАнонимный тип по образцу Vector:");
        var anonymousVector = new
        {
            ID = 9999,
            Size = 2,
            Values = new int[] { 11, 22 },
            Version = Vector.Version,
            IsAnonymous = true
        };

        Console.WriteLine($"Анонимный объект (ToString): {anonymousVector}");
        Console.WriteLine($"Тип анонимного объекта: {anonymousVector.GetType()}");
    }
}