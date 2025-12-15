using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Globalization;

public interface ILoggable
{
    void LogInfo();
}

public class Product : ILoggable
{
    public string Category = "Electronics";
    private double internalCost;
    public int ProductId { get; set; }
    public string Name { get; private set; }

    public Product()
    {
        ProductId = 0;
        Name = "Unknown Product";
    }

    public Product(int id, string name, double cost)
    {
        ProductId = id;
        Name = name;
        internalCost = cost;
    }

    public void UpdatePrice(double newPrice)
    {
        internalCost = newPrice;
        Console.WriteLine($"[Product] Цена продукта {Name} обновлена до {internalCost}");
    }

    public string GetInfo(int quantity)
    {
        return $"Product: {Name}, ID: {ProductId}, Qty: {quantity}";
    }

    public void LogInfo()
    {
        Console.WriteLine($"Logging: Product {Name} created.");
    }
}

public class Customer
{
    public int CustomerId { get; set; }
    public string FullName { get; set; }

    public Customer(int id, string name)
    {
        CustomerId = id;
        FullName = name;
    }

    public void PlaceOrder(string itemName, int quantity)
    {
        System.Console.WriteLine($"[Customer] Заказ от {FullName}: {quantity} x {itemName}");
    }
}


public static class Reflector
{
    public static readonly string LogFilePath = "ReflectionLog.txt";

    private static Type GetClassType(string className)
    {
        Type type = Type.GetType(className) ??
                    Assembly.GetExecutingAssembly().GetTypes()
                            .FirstOrDefault(t => t.Name == className);

        if (type == null)
        {
            throw new ArgumentException($"Тип с именем '{className}' не найден.");
        }
        return type;
    }

    private static void WriteToFile(string header, IEnumerable<string> data, string format = "text")
    {
        using (StreamWriter sw = new StreamWriter(LogFilePath, true, Encoding.UTF8))
        {
            sw.WriteLine($"{header} ");

            if (format.ToLower() == "json")
            {
                string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                sw.WriteLine(jsonString);
            }
            else
            {
                foreach (var item in data)
                {
                    sw.WriteLine(item);
                }
            }
            sw.WriteLine("\n");
        }
    }

    public static void GetAssemblyName(string className)
    {
        try
        {
            Type type = GetClassType(className);
            string assemblyName = type.Assembly.FullName;
            WriteToFile($"1.a. Имя сборки для класса {className}", new[] { assemblyName });
        }
        catch (Exception ex)
        {
            WriteToFile($"1.a. Ошибка при получении имени сборки для {className}", new[] { ex.Message });
        }
    }

    public static void HasPublicConstructors(string className)
    {
        try
        {
            Type type = GetClassType(className);
            bool hasPublic = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Any();
            WriteToFile($"1.b. Наличие публичных конструкторов для класса {className}", new[] { $"Результат: {hasPublic}" });
        }
        catch (Exception ex)
        {
            WriteToFile($"1.b. Ошибка при проверке конструкторов для {className}", new[] { ex.Message });
        }
    }

    public static IEnumerable<string> GetPublicMethods(string className)
    {
        try
        {
            Type type = GetClassType(className);
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                              .Select(m => $"[Method] {m.ReturnType.Name} {m.Name}({string.Join(", ", m.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"))})");

            WriteToFile($"1.c. Публичные методы класса {className} (только объявленные)", methods, "json");
            return methods;
        }
        catch (Exception ex)
        {
            WriteToFile($"1.c. Ошибка при получении методов для {className}", new[] { ex.Message });
            return Enumerable.Empty<string>();
        }
    }

    public static IEnumerable<string> GetFieldsAndProperties(string className)
    {
        try
        {
            Type type = GetClassType(className);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                             .Select(f => $"[Field] Access: {GetAccessModifier(f)} Type: {f.FieldType.Name} Name: {f.Name}");

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                                 .Select(p => $"[Property] Type: {p.PropertyType.Name} Name: {p.Name} (Read: {p.CanRead}, Write: {p.CanWrite})");

            var result = fields.Concat(properties);
            WriteToFile($"1.d. Поля и свойства класса {className}", result);
            return result;
        }
        catch (Exception ex)
        {
            WriteToFile($"1.d. Ошибка при получении полей/свойств для {className}", new[] { ex.Message });
            return Enumerable.Empty<string>();
        }
    }

    private static string GetAccessModifier(FieldInfo f)
    {
        if (f.IsPublic) return "public";
        if (f.IsPrivate) return "private";
        if (f.IsFamily) return "protected";
        return "internal/other";
    }

    public static IEnumerable<string> GetInterfaces(string className)
    {
        try
        {
            Type type = GetClassType(className);
            var interfaces = type.GetInterfaces().Select(i => i.FullName);

            WriteToFile($"1.e. Интерфейсы класса {className}", interfaces);
            return interfaces;
        }
        catch (Exception ex)
        {
            WriteToFile($"1.e. Ошибка при получении интерфейсов для {className}", new[] { ex.Message });
            return Enumerable.Empty<string>();
        }
    }

    public static void GetMethodsByParameterType(string className, Type parameterType)
    {
        try
        {
            Type type = GetClassType(className);
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                              .Where(m => m.GetParameters().Any(p => p.ParameterType == parameterType))
                              .Select(m => m.Name)
                              .Distinct();

            WriteToFile($"1.f. Методы класса {className}, содержащие параметр типа {parameterType.Name}", methods);
        }
        catch (Exception ex)
        {
            WriteToFile($"1.f. Ошибка при поиске методов по типу параметра для {className}", new[] { ex.Message });
        }
    }

    public static object InvokeMethod(object obj, string methodName, object[] parameters = null, string parameterSource = "file")
    {
        Type type = obj.GetType();
        MethodInfo method = type.GetMethod(methodName);

        if (method == null)
        {
            WriteToFile($"1.g. Ошибка вызова {methodName} на {type.Name}", new[] { $"Метод '{methodName}' не найден." });
            return null;
        }

        ParameterInfo[] methodParams = method.GetParameters();
        object[] args = parameters;

        if (args == null)
        {
            if (parameterSource.ToLower() == "file")
            {
                args = ReadParametersFromFile(methodParams);
            }
            else if (parameterSource.ToLower() == "generate")
            {
                args = GenerateParameters(methodParams);
            }
        }

        if (args != null && args.Length != methodParams.Length)
        {
            WriteToFile($"1.g. Ошибка вызова {methodName} на {type.Name}", new[] { $"Неверное количество аргументов: Ожидалось {methodParams.Length}, получено {args.Length}" });
            return null;
        }

        try
        {
            object result = method.Invoke(obj, args);
            WriteToFile($"1.g. Вызов метода {methodName} на объекте {type.Name}", new[] { $"Источник параметров: {parameterSource}. Успешно вызван. Возвращаемое значение: {(result ?? "void/null")}" });
            return result;
        }
        catch (Exception ex)
        {
            WriteToFile($"1.g. Ошибка при вызове метода {methodName}", new[] { $"Исключение: {ex.InnerException?.Message ?? ex.Message}" });
            return null;
        }
    }

    private static object[] GenerateParameters(ParameterInfo[] parameters)
    {
        var generatedArgs = new List<object>();
        foreach (var param in parameters)
        {
            if (param.ParameterType == typeof(int))
            {
                generatedArgs.Add(10);
            }
            else if (param.ParameterType == typeof(string))
            {
                generatedArgs.Add("Generated Item");
            }
            else if (param.ParameterType == typeof(double))
            {
                generatedArgs.Add(50.5);
            }
            else
            {
                try
                {
                    generatedArgs.Add(Activator.CreateInstance(param.ParameterType));
                }
                catch
                {
                    generatedArgs.Add(null);
                }
            }
        }
        return generatedArgs.ToArray();
    }

    private static object[] ReadParametersFromFile(ParameterInfo[] parameters)
    {
        if (!File.Exists("input.txt"))
        {
            throw new FileNotFoundException("Файл 'input.txt' не найден для чтения параметров.");
        }

        var fileParams = new List<object>();
        var lines = File.ReadAllLines("input.txt");
        int lineIndex = 0;

        foreach (var param in parameters)
        {
            if (lineIndex >= lines.Length)
            {
                throw new InvalidOperationException("Недостаточно параметров в файле input.txt");
            }

            string line = lines[lineIndex].Trim();
            object value;

            try
            {
                value = Convert.ChangeType(line, param.ParameterType, CultureInfo.InvariantCulture);
                fileParams.Add(value);
            }
            catch
            {
                throw new InvalidCastException($"Не удалось преобразовать '{line}' в тип {param.ParameterType.Name} для параметра {param.Name}.");
            }
            lineIndex++;
        }
        return fileParams.ToArray();
    }

    public static T Create<T>(params object[] args)
    {
        Type type = typeof(T);
        Type[] argTypes = args.Select(a => a?.GetType()).ToArray();

        ConstructorInfo constructor = type.GetConstructor(argTypes);

        if (constructor == null)
        {
            constructor = type.GetConstructors()
                              .FirstOrDefault(c => c.GetParameters().Length == args.Length);
        }

        if (constructor == null)
        {
            throw new InvalidOperationException($"2. Не удалось найти подходящий публичный конструктор для типа {type.Name} с {args.Length} параметрами.");
        }

        try
        {
            T instance = (T)constructor.Invoke(args);
            WriteToFile($"2. Создание объекта типа {type.Name}", new[] { $"Объект успешно создан с помощью Reflector.Create<{type.Name}>()." });
            return instance;
        }
        catch (Exception ex)
        {
            WriteToFile($"2. Ошибка при создании объекта типа {type.Name}", new[] { $"Исключение: {ex.InnerException?.Message ?? ex.Message}" });
            throw;
        }
    }
}


public class Program
{
    public static void Main(string[] args)
    {
        if (File.Exists(Reflector.LogFilePath))
        {
            File.WriteAllText(Reflector.LogFilePath, string.Empty);
        }

        Console.WriteLine("Результаты записываются в ReflectionLog.txt...\n");

        Product p1;
        Customer c1;

        try
        {
            Console.WriteLine("2. Демонстрация метода Create<T>()");

            p1 = Reflector.Create<Product>();
            Console.WriteLine($"- Создан P1 (Product): {p1.Name} (ID: {p1.ProductId})");

            c1 = Reflector.Create<Customer>(101, "Alice Smith");
            Console.WriteLine($"- Создан C1 (Customer): {c1.FullName} (ID: {c1.CustomerId})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка при создании: {ex.Message}");
            return;
        }

        DemonstrateClassReflection("Product");
        DemonstrateClassReflection("Customer");
        DemonstrateClassReflection("System.DateTime");

        Console.WriteLine("\n1.f. Поиск методов по типу параметра (Int32, String)...");
        Reflector.GetMethodsByParameterType("Product", typeof(int));
        Reflector.GetMethodsByParameterType("Customer", typeof(string));

        try
        {
            Console.WriteLine("1.g. Демонстрация метода Invoke");

            Console.WriteLine("-> Вызов UpdatePrice на P1 с параметром из файла (input.txt)");
            Reflector.InvokeMethod(p1, "UpdatePrice", parameterSource: "file");

            Console.WriteLine("\n-> Вызов PlaceOrder на C1 с сгенерированными параметрами");
            Reflector.InvokeMethod(c1, "PlaceOrder", parameterSource: "generate");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка при Invoke: {ex.Message}");
        }

        Console.WriteLine("\nРезультат выполнения кода находится в ReflectionLog.txt");
    }

    private static void DemonstrateClassReflection(string className)
    {
        Console.WriteLine($"\nИсследование класса: {className}");
        Reflector.GetAssemblyName(className);
        Reflector.HasPublicConstructors(className);
        Reflector.GetPublicMethods(className);
        Reflector.GetFieldsAndProperties(className);
        Reflector.GetInterfaces(className);
    }
}