using System;
using System.Linq;
using System.Text;

public partial class Vector
{
    private readonly int _id;
    private int[] _elements;
    public const string CLASS_DESCRIPTION = "Это класс для работы с целочисленным вектором.";
    private static int _objectCount = 0;

    public int Size
    {
        get { return _elements.Length; }
    }

    public int Id
    {
        get { return _id; }
    }

    public int ErrorCode
    {
        get;
        private set;
    }

    public int this[int index]
    {
        get
        {
            if (index >= 0 && index < Size)
            {
                ErrorCode = 0;
                return _elements[index];
            }
            else
            {
                ErrorCode = -1;
                return 0;
            }
        }
        set
        {
            if (index >= 0 && index < Size)
            {
                _elements[index] = value;
                ErrorCode = 0; 
            }
            else
            {
                ErrorCode = -2;
            }
        }
    }

    static Vector()
    {
        _objectCount = 0;
        Console.WriteLine("Статический конструктор Vector вызван.");
    }

    public Vector()
    {
        _elements = new int[0];
        _id = this.GetHashCode();
        _objectCount++;
    }

    public Vector(int size)
    {
        if (size < 0)
        {
            size = 0;
            ErrorCode = -3;
        }
        _elements = new int[size];
        _id = size.GetHashCode();
        _objectCount++;
    }

    public Vector(int[] initialElements, int customId = 0)
    {
        if (initialElements != null)
        {
            _elements = initialElements;
        }
        else
        {
            _elements = new int[0];
        }
        _id = _elements.Length.GetHashCode() ^ customId.GetHashCode();
        _objectCount++;
    }

    private Vector(int fixedId, bool isPrivate)
    {
        _elements = new int[] { fixedId };
        _id = fixedId;
        _objectCount++;
        Console.WriteLine("Вызван закрытый конструктор.");
    }
    
    public static Vector CreateVectorWithFixedId(int id)
    {
        return new Vector(id, true);
    }

    public void GetSumAndProduct(ref int sum, out int product)
    {
        product = 1;
        for (int i = 0; i < Size; i++)
        {
            sum += _elements[i];
            product *= _elements[i];
        }
    }

    public bool Contains(int value)
    {
        foreach (int element in _elements)
        {
            if (element == value) return true;
        }
        return false;
    }

    public static void DisplayClassInfo()
    {
        Console.WriteLine("\n--- Информация о классе Vector ---");
        Console.WriteLine(CLASS_DESCRIPTION);
        Console.WriteLine($"Количество созданных объектов: {_objectCount}");
        Console.WriteLine("---------------------------------");
    }

    public override bool Equals(object obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
        {
            return false;
        }
        Vector otherVector = (Vector)obj;
        if (this.Size != otherVector.Size)
        {
            return false;
        }
        for (int i = 0; i < this.Size; i++)
        {
            if (this._elements[i] != otherVector._elements[i])
            {
                return false;
            }
        }
        return true;
    }

    public override int GetHashCode()
    {
        int hash = 17;
        hash = hash * 23 + _id.GetHashCode();
        hash = hash * 23 + Size.GetHashCode();
        return hash;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"Vector (ID: {_id}, Size: {Size}, Error: {ErrorCode}) [");
        sb.Append(string.Join(", ", _elements));
        sb.Append("]");
        return sb.ToString();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("---------- Задание 2: Создание и проверка объектов ----------");

        Vector v1 = new Vector(new int[] { 10, 20, 0, 40 });
        Vector v2 = new Vector(4);
        Vector v3 = new Vector();
        Vector v4 = new Vector(new int[] { 10, 20, 0, 40 });
        Vector v5 = Vector.CreateVectorWithFixedId(777);

        Console.WriteLine($"v1: {v1}");
        Console.WriteLine($"v2: {v2}");
        Console.WriteLine($"v3: {v3}");
        Console.WriteLine($"v4: {v4}");
        Console.WriteLine($"v5: {v5}");

        Console.WriteLine($"\nРазмер v1: {v1.Size}");
        Console.WriteLine($"Третий элемент v1: {v1[2]}");
        v1[0] = 15;
        Console.WriteLine($"Измененный v1: {v1}");

        int nonExistentElement = v1[100];
        Console.WriteLine($"Попытка чтения v1[100]. Значение: {nonExistentElement}, Код ошибки: {v1.ErrorCode}");

        Console.WriteLine($"\nСравнение v1 и v4 (Equals): {v1.Equals(v4)}");
        Console.WriteLine($"Сравнение v2 и v4 (Equals): {v2.Equals(v4)}");

        Console.WriteLine($"Тип объекта v1: {v1.GetType()}");

        int sum = 100;
        int product;
        v1.GetSumAndProduct(ref sum, out product);
        Console.WriteLine($"\nСумма элементов v1 + 100 = {sum}");
        Console.WriteLine($"Произведение элементов v1 = {product}");

        Vector.DisplayClassInfo();

        Console.WriteLine("\n---------- Задание 3: Работа с массивом объектов ----------");

        Vector[] vectors = {
            new Vector(new int[] {1, -2, 3}),
            new Vector(new int[] {5, 8}),
            new Vector(new int[] {10, 0, -5}),
            new Vector(new int[] {-1, -1}),
            new Vector(new int[] {0, 12, 15}),
            new Vector(new int[] {3, 4})
        };

        Console.WriteLine("\nа) Список векторов, содержащих 0:");
        foreach (var vec in vectors)
        {
            if (vec.Contains(0)) Console.WriteLine(vec);
        }

        Console.WriteLine("\nb) Вектор(ы) с наименьшим модулем:");
        if (vectors.Length > 0)
        {
            double minMagnitude = vectors.Min(v => v.GetMagnitude());
            Console.WriteLine($"Наименьший модуль: {minMagnitude:F2}");
            foreach (var vec in vectors)
            {
                if (Math.Abs(vec.GetMagnitude() - minMagnitude) < 1e-9)
                {
                    Console.WriteLine(vec);
                }
            }
        }

        Console.WriteLine("\n---------- Задание 4: Анонимный тип ----------");

        var anonymousVector = new
        {
            Id = v1.Id,
            Size = v1.Size,
            Elements = new int[] { 1, 1, 2, 3, 5 },
            Description = "Анонимный тип, похожий на Vector"
        };

        Console.WriteLine($"\nИнформация об анонимном типе:");
        Console.WriteLine($"ID: {anonymousVector.Id}, Размер: {anonymousVector.Size}");
        Console.WriteLine($"Описание: {anonymousVector.Description}");
        Console.WriteLine($"Элементы: [{string.Join(", ", anonymousVector.Elements)}]");
    }
}
