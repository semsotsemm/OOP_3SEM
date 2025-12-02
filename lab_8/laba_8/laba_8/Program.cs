using System.Text;

public delegate void MoveDelegate(double offsetX, double offsetY);
public delegate void CompressDelegate(double factor);

public class User
{
    public event MoveDelegate? Move;
    public event CompressDelegate? Compress;

    public void OnMove(double offsetX, double offsetY)
    {
        Console.WriteLine($"\nПользователь перемещает объект на ({offsetX:F2}, {offsetY:F2})");
        Move?.Invoke(offsetX, offsetY);
    }

    public void OnCompress(double factor)
    {
        Console.WriteLine($"\n Пользователь сжимает объект. Коэффициент сжатия: {factor:F2} ");
        Compress?.Invoke(factor);
    }
}

public abstract class Shape
{
    public string Name { get; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Size { get; protected set; }

    public Shape(string name, double x, double y, double size)
    {
        Name = name;
        X = x;
        Y = y;
        Size = size;
    }

    public void MoveHandler(double offsetX, double offsetY)
    {
        X += offsetX;
        Y += offsetY;
        Console.WriteLine($"[{Name}] ПЕРЕМЕЩЕН: Новые координаты ({X:F2}, {Y:F2})");
    }

    public abstract void CompressHandler(double factor);

    public override string ToString() 
    { 
        return $"{Name} (X:{X:F2}, Y:{Y:F2}, Размер:{Size:F2})";
    }
}

public class Circle : Shape
{
    public Circle(string name, double x, double y, double radius) : base(name, x, y, radius) { }

    public override void CompressHandler(double factor)
    {
        Size /= factor; 
        Console.WriteLine($"[{Name}] СЖАТ: Новый радиус {Size:F2}");
    }
}

public class Square : Shape
{
    public Square(string name, double x, double y, double side) : base(name, x, y, side) { }

    public override void CompressHandler(double factor)
    {
        Size /= Math.Sqrt(factor);
        Console.WriteLine($"[{Name}] СЖАТ: Новая сторона {Size:F2}");
    }
}

public static class StringProcessor
{
    public static string RemovePunctuation(string input)
    {
        var result = new StringBuilder();
        input.Where(c => !char.IsPunctuation(c)).ToList().ForEach(c => result.Append(c));
        return result.ToString();
    }
    public static string ToUpperCase(string input) => input.ToUpper();

    public static string RemoveExtraSpaces(string input)
    {
        return string.Join(" ", input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
    }

    public static string AddPrefix(string input) => $"[--СТАРТ--] {input}";
    public static string ReplaceA(string input) => input.Replace('a', '@').Replace('A', '@');
}

class Program
{
    static void RunPart1DelegatesAndEvents()
    {
        User user = new User();
        Circle circle1 = new Circle("Круг-A", 0, 0, 5.0);
        Circle circle2 = new Circle("Круг-B", 10, 5, 3.0);
        Square square1 = new Square("Квадрат-A", -5, -5, 8.0);
        Square square2 = new Square("Квадрат-B", 20, 10, 2.0);

        Console.WriteLine("1. Исходные состояния объектов");
        Console.WriteLine(circle1);
        Console.WriteLine(circle2);
        Console.WriteLine(square1);
        Console.WriteLine(square2);

        user.Move += circle1.MoveHandler;
        user.Compress += circle1.CompressHandler;

        user.Move += (ox, oy) =>
        {
            circle2.X += ox;
            circle2.Y += oy;
            Console.WriteLine($"[Круг-B] ПЕРЕМЕЩЕН: Новые координаты ({circle2.X:F2}, {circle2.Y:F2})");
        };

        user.Compress += square1.CompressHandler;

        user.OnMove(10.5, -2.1);
        user.OnCompress(2.0); 

        Console.WriteLine("\n2. Конечные состояния объектов");
        Console.WriteLine(circle1); 
        Console.WriteLine(circle2); 
        Console.WriteLine(square1); 
        Console.WriteLine(square2); 
    }

    static void RunPart2StringProcessing()
    {
        string originalString = "    ЭтО; прИмЕр  обработки, строки.  Aaaaaa! ";
        Console.WriteLine($"\n Исходная строка: \"{originalString}\" ");

        var processingSteps = new List<Func<string, string>>
        {
            StringProcessor.RemovePunctuation, 
            StringProcessor.ToUpperCase,       
            StringProcessor.RemoveExtraSpaces, 
            StringProcessor.ReplaceA,          
            StringProcessor.AddPrefix          
        };

        string finalString = originalString;

        Console.WriteLine("\nАлгоритм последовательной обработки ");
        foreach (var step in processingSteps)
        {
            finalString = step(finalString);
            Console.WriteLine($" Шаг {processingSteps.IndexOf(step) + 1} ({step.Method.Name}): \"{finalString}\"");
        }

        Console.WriteLine($"\n КОНЕЧНЫЙ РЕЗУЛЬТАТ: \"{finalString}\" ");
    }

    static void Main(string[] args)
    {
        RunPart1DelegatesAndEvents();

        Console.WriteLine("\n----------------------------------------------------");

        RunPart2StringProcessing();
    }
}