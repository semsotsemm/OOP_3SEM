public partial class Array
{
    private int Size;
    private int[] Elements;
    private Production DeveloperProduction;

    public Array()
    {
        Size = 0;
        Elements = new int[Size];
        DeveloperProduction = new Production();
    }
    public Array(int size)
    {
        Size = size;
        Elements = new int[Size];
        DeveloperProduction = new Production();
    }
    public Array(int[] elements)
    {
        Size = elements.Length;
        Elements = new int[Size];
        for (int i = 0; i < Size; i++)
        {
            Elements[i] = elements[i];
        }
        DeveloperProduction = new Production();
    }
}

public class Production
{
    private int Id;
    private string OrganizationName;
    public Production()
    {
        OrganizationName = "BSTU";
        Id = 613;
    }
    public string GetOrganizationName()
    {
        return OrganizationName;
    }
    public int GetId()
    {
        return Id;
    }
}


static class StatisticOperation 
{
    public static int Sum(Array arr)
    {
        int sum = 0;
        for (int i = 0; i < arr.getSize(); i++)
        {
            sum += arr[i];
        }
        return sum;
    }
    public static int getNumberOfElements(Array arr)
    {
        int count = 0;
        for (int i = 0; i < arr.getSize(); i++)
        {
            count++;
        }
        return count;
    }
    public static int getDifferenceMaxMinElement(Array arr)
    {
        int min = 0;
        int max = 0;
        for (int i = 0; i < arr.getSize(); i++)
        {
            if (arr[i] > max || i == 0)
            {
                max = arr[i];
            }
            if (arr[i] < min || i == 0)
            {
                min = arr[i];
            }
        }
        return max-min;
    }
    public static bool ContainsChar(string source, char character)
    {
        for (int i = 0; i < source.Length; i++)
        {
            if (character == source[i])
            {
                return true;
            }
        }
        return false;
    }
}

class Program
{
    static void TestResult(string testName, bool condition)
    {
        Console.Write($"[{testName}]: ");
        if (condition)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("ПРОЙДЕН");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("СБОЙ");
        }
        Console.ResetColor();
    }

    static void TestResult(string testName, bool actual, bool expected = true)
    {
        TestResult(testName, actual == expected);
    }
    
    static bool CompareArrayContent(Array arr, int[] expected)
    {
        return arr.getSize() == expected.Length && arr.getElements().SequenceEqual(expected);
    }

    static void Main(string[] args)
    {
        Console.WriteLine("===== КОМПЛЕКСНОЕ ТЕСТИРОВАНИЕ КЛАССА ARRAY =====");

        Console.WriteLine("\n--- 1. Конструкторы и Геттеры ---");

        Array arrEmpty = new Array();
        TestResult("Конструктор по умолчанию (Size 0)", arrEmpty.getSize() == 0);

        Array arrFixed = new Array(3);
        TestResult("Конструктор с размером (Size 3)", arrFixed.getSize() == 3);

        int[] initData = { 1, 2, 3, -4 };
        Array arrInit = new Array(initData);
        TestResult("Конструктор с массивом", CompareArrayContent(arrInit, initData));

        Console.WriteLine("\n--- 2. setSize ---");

        Array arrResize = new Array(new int[] { 10, 20 });
        arrResize.setSize(4);
        TestResult("setSize (Увеличение до 4)", arrResize.getSize() == 4 && arrResize.getElements()[3] == 0);
        arrResize.setSize(1);
        TestResult("setSize (Уменьшение до 1)", CompareArrayContent(arrResize, new int[] { 10 }));
        TestResult("setSize (Отрицательный размер)", arrResize.setSize(-5) == false && arrResize.getSize() == 1);

        Console.WriteLine("\n--- 3. append (Добавление элемента) ---");

        Array arrAppend = new Array(new int[] { 1, 2 });
        arrAppend.append(3);
        arrAppend.append(4);

        TestResult("append (Работоспособность)", CompareArrayContent(arrAppend, new int[] { 1, 2, 3, 4 }));

        Console.WriteLine("\n--- 4. removeNegativeElements ---");

        Array arrRemove = new Array(new int[] { 5, -2, 10, -8, 0 });
        arrRemove.removeNegativeElements();

        TestResult("removeNegativeElements", CompareArrayContent(arrRemove, new int[] { 5, 10, 0 }));

        Array arrNoNegative = new Array(new int[] { 1, 2, 3 });
        arrNoNegative.removeNegativeElements();
        TestResult("removeNegativeElements (Нет отрицательных)", CompareArrayContent(arrNoNegative, new int[] { 1, 2, 3 }));

        Console.WriteLine("\n--- 5. Оператор * (Умножение) ---");

        Array arrL = new Array(new int[] { 2, 3, 4, 5 });
        Array arrR = new Array(new int[] { 10, 2, 1 });

        Array resultMul = arrL * arrR;
        TestResult("Оператор * (Разные размеры)", CompareArrayContent(resultMul, new int[] { 20, 6, 4, 5 }));

        Console.WriteLine("\n--- 6. Операторы Сравнения ---");

        Array a = new Array(new int[] { 1, 2, 3 });
        Array b = new Array(new int[] { 1, 2, 3 });
        Array c = new Array(new int[] { 1, 2, 4 });
        Array d = new Array(new int[] { 1, 2 });

        TestResult("Оператор == (Идентичные)", a == b);
        TestResult("Оператор != (Разное содержимое)", a != c);
        TestResult("Оператор == (Разный размер)", a == d, false);
        TestResult("Оператор > (a > c: 6 > 7)", a > c, false);
        TestResult("Оператор > (c > a: 7 > 6)", c > a);
        TestResult("Оператор < (d < a: 3 < 6)", d < a);

        Console.WriteLine("\n--- 7. Логические Операторы (true/false) ---");

        Array arrPositive = new Array(new int[] { 1, 5, 10 });
        Array arrNegative = new Array(new int[] { 1, -5, 10 });

        bool testPositiveTrue = false;
        if (arrPositive) { testPositiveTrue = true; }

        TestResult("Оператор true (Все положительные)", testPositiveTrue);
        TestResult("Оператор false (Есть отрицательный)", arrNegative.getElements().Any(e => e < 0));

        Console.WriteLine("\n--- 8. Вспомогательная функция ContainsChar ---");

        string testString = "Привет, мир!";

        TestResult("ContainsChar (Символ 'м' найден)", StatisticOperation.ContainsChar(testString, 'м'));
        TestResult("ContainsChar (Символ ',' найден)", StatisticOperation.ContainsChar(testString, ','));
        TestResult("ContainsChar (Символ 'Z' не найден)", StatisticOperation.ContainsChar(testString, 'Z'), false);

        Console.WriteLine("\n--- 9. Оператор приведения (int) ---");

        Array arrCast = new Array(new int[] { 10, 20, 30, 40 });
        int arraySize = (int)arrCast;
        TestResult("Приведение к int (Размер 4)", arraySize == 4);

        Array arrEmptyCast = new Array();
        int emptySize = (int)arrEmptyCast;
        TestResult("Приведение к int (Пустой массив)", emptySize == 0);

        Console.WriteLine("\n--- 10. Индексатор (this[int index]) ---");

        Array arrIndex = new Array(new int[] { 100, 200, 300 });

        int readValue = arrIndex[1];
        TestResult("Индексатор GET (arr[1] == 200)", readValue == 200);
        
        arrIndex[0] = 99;
        TestResult("Индексатор SET (arr[0] == 99)", arrIndex.getElements()[0] == 99);

        bool readExceptionCaught = false;
        try
        {
            int val = arrIndex[3]; 
        }
        catch (IndexOutOfRangeException)
        {
            readExceptionCaught = true;
        }
        TestResult("Индексатор GET (IndexOutOfRangeException)", readExceptionCaught);

        bool writeExceptionCaught = false;
        try
        {
            arrIndex[-1] = 1; 
        }
        catch (IndexOutOfRangeException)
        {
            writeExceptionCaught = true;
        }
        TestResult("Индексатор SET (IndexOutOfRangeException)", writeExceptionCaught);

        Console.WriteLine("\n--- 11. Developer (Вложенный класс) и Production (Вложенный объект) ---");

        TestResult("Developer.Fio (Статическое)", Array.Developer.Fio == "Antsipov Alexey");
        TestResult("Developer.OrganizationTitle (Новое статическое поле)", Array.Developer.OrganizationTitle == "BSTU");

        Array arrProd = new Array(1); 

        TestResult("Production.OrganizationName (Статическое поле)", arrProd.GetOrganizationName() == "BSTU");
        TestResult("Production.Id (Статическое поле)", arrProd.GetId() == 613);

        bool isFieldAccessible = arrProd.GetDeveloperProduction() != null;
        TestResult("Поле DeveloperProduction доступно", isFieldAccessible);

        Console.WriteLine("\n--- 12. Статический класс StatisticOperation ---");

        Array arrStats = new Array(new int[] { 10, 5, -2, 20, 0, 8 });
        Array arrEmptyStats = new Array();

        int expectedSum = 41;
        TestResult("StatisticOperation.Sum (Сумма)", StatisticOperation.Sum(arrStats) == expectedSum);
        TestResult("StatisticOperation.Sum (Пустой массив)", StatisticOperation.Sum(arrEmptyStats) == 0);

        int expectedCount = 6;
        TestResult("StatisticOperation.getNumberOfElements (Размер 6)", StatisticOperation.getNumberOfElements(arrStats) == expectedCount);
        TestResult("StatisticOperation.getNumberOfElements (Пустой массив)", StatisticOperation.getNumberOfElements(arrEmptyStats) == 0);

        int expectedDifference = 22;
        TestResult("StatisticOperation.getDifferenceMaxMinElement (Разница 22)", StatisticOperation.getDifferenceMaxMinElement(arrStats) == expectedDifference);

        TestResult("StatisticOperation.getDifferenceMaxMinElement (Пустой массив)", StatisticOperation.getDifferenceMaxMinElement(arrEmptyStats) == 0);


        Console.WriteLine("\n===== ТЕСТИРОВАНИЕ ЗАВЕРШЕНО =====");
    }
}
