using System;
using System.Runtime.InteropServices;

class Table
{
    private string name = "Таблица";
    private int colums = 3;
    private int rows = 0;
    private int colum_width = 15;
    private string[] name_of_colums = { "Тип данных", "Занимаемое место(бит)", "Инициализированное значением" };
    private string[][] values = new string[0][];

    public Table(string table_name)
    {
        name = table_name;
    }

    public void add_value(Type type_value, object value)
    {
        string[][] newValues = new string[rows + 1][];

        for (int i = 0; i < rows; i++)
        {
            newValues[i] = values[i];
        }

        string size;
        try
        {
            size = Marshal.SizeOf(type_value).ToString();
        }
        catch
        {
            size = IntPtr.Size.ToString(); 
        }

        newValues[rows] = new string[colums];
        newValues[rows][0] = type_value.Name;
        newValues[rows][1] = size;
        newValues[rows][2] = value.ToString();

        values = newValues;
        rows++;
    }

    public void set_color(string size_line)
    {
        int size = Convert.ToInt32(size_line);
        if (size <= 2)
        {
            Console.ForegroundColor = ConsoleColor.Green;
        }
        else if (size < 7)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }
    }
    public void print_table()
    {

        Console.WriteLine("{0," + (4 * colum_width - name.Length / 2) + "}", name);
        for (int i = 0; i < colums; i++)
        {
            Console.Write("{0," + (colum_width - name_of_colums[i].Length / 2) + "}", " ");
            Console.Write("{0,-" + (colum_width + name_of_colums[i].Length / 2) + "}", name_of_colums[i]);
            Console.Write("|");
        }
        for (int i = 0; i < rows; i++)
        {
            Console.WriteLine();
            for (int j = 0; j < colums; j++)
            {
                Console.Write("{0," + (colum_width - values[i][j].Length / 2) + "}", " ");
                if (j == 1) 
                {
                    set_color(values[i][j]);
                }
                Console.Write("{0,-" + (colum_width + values[i][j].Length / 2) + "}", values[i][j]);
                if (j == 1)
                {
                    Console.ResetColor();
                }
                Console.Write("|");
            }
        }
    }
}

class lab_1
{
    public static void Main()
    {
        //===============1.a===============
        Table table = new Table("Моя таблица");
        bool parm_bool = true;
        byte parm_byte = 200;
        sbyte parm_sbyte = 17;
        char parm_char = 's';
        decimal parm_decimal = 2.1m;
        double parm_double = 3.14;
        float parm_float = 2.17f;
        int parm_int = 42;
        uint parm_uint = 52;
        long parm_long = 5239;
        ulong parm_ulong = 522478247;
        short parm_short = 17;
        ushort parm_ushort = 21;
        string parm_string = "hello";
        table.add_value(parm_bool.GetType(), parm_bool);
        table.add_value(parm_byte.GetType(), parm_byte);
        table.add_value(parm_sbyte.GetType(), parm_sbyte);
        table.add_value(parm_char.GetType(), parm_char);
        table.add_value(parm_decimal.GetType(), parm_decimal);
        table.add_value(parm_double.GetType(), parm_double);
        table.add_value(parm_float.GetType(), parm_float);
        table.add_value(parm_int.GetType(), parm_int);
        table.add_value(parm_uint.GetType(), parm_uint);
        table.add_value(parm_long.GetType(), parm_long);
        table.add_value(parm_ulong.GetType(), parm_ulong);
        table.add_value(parm_short.GetType(), parm_short);
        table.add_value(parm_ushort.GetType(), parm_ushort);
        table.add_value(parm_string.GetType(), parm_string);
        table.print_table();

        //===============1.b===============

        //Неявное приведение
        int a = 10;
        long implicit_long= a;              
        float implicit_float = a;             
        double implicit_double = implicit_float;            
        char symbol = 'A';
        int code = symbol;          
        byte x = 42;
        double y = x;

        //Явное приведение
        double e = 9.78;
        int f = (int)e;         
        long g = 1000;
        short h = (short)g;    
        float i = 3.14f;
        int j = (int)i;         
        decimal k = 123.45m;
        double l = (double)k;   
        int n = 300;
        byte m = (byte)n;

        //Использование Convert
        string text = "123";
        int num = Convert.ToInt32(text);       
        double dnum = Convert.ToDouble("3,14");
        bool flag = Convert.ToBoolean("True"); 
        string s = Convert.ToString(42);      
        char c = Convert.ToChar("A");

        //===============1.c===============
        Console.WriteLine("\n---------------------------------------------------------------------------------------------");
        int significant_int = 12;
        object packed_value = significant_int;
        int unpacked_int = (int)packed_value;
        Console.WriteLine("Значение int: до упаковки {0}, после распоковки {1} ", significant_int, unpacked_int);
        string significant_string = "hello";
        packed_value = significant_string;
        string unpacked_string = (string)packed_value;
        Console.WriteLine("Значение string: до упаковки {0}, после распоковки {1} ", significant_string, unpacked_string);
        string significant_char = "i";
        packed_value = significant_char;
        string unpacked_char = (string)packed_value;
        Console.WriteLine("Значение string: до упаковки {0}, после распоковки {1} ", significant_char, unpacked_char);

        //===============1.d===============
        Console.WriteLine("\n---------------------------------------------------------------------------------------------");
        var number = 10;             // int
        var name = "Alice";          // string
        var price = 99.99;           // double
        var isActive = true;         // bool
        var items = new[] { 1, 2, 3 }; // int[]
        Console.WriteLine("Неявная типизация {0}, тип данных: {1}", number, number.GetType());
        Console.WriteLine("Неявная типизация {0}, тип данных: {1}", name, name.GetType());
        Console.WriteLine("Неявная типизация {0}, тип данных: {1}", price, price.GetType());
        Console.WriteLine("Неявная типизация {0}, тип данных: {1}", isActive, isActive.GetType());
        Console.WriteLine("Неявная типизация {0}, тип данных: {1}", items, items.GetType());

        //===============1.e===============
        Console.WriteLine("\n---------------------------------------------------------------------------------------------");
        int? age = null; // Nullable позволяет присвоить значимым типам null (без ? была бы ошибка, null можно присваивать только ссылкам )
        if (age.HasValue)
        {
            Console.WriteLine("Возраст: {0}", age.Value);
        }
        else 
        {
            Console.WriteLine("Возраст не задан");
        }

        //===============1.d===============
        Console.WriteLine("\n---------------------------------------------------------------------------------------------");
        var int_through_var = 5;
        // int_through_var = "hello" var это не динамический тип, а просто способ неявно указать тип переменной (после выполнения предыдущей строчки тип данных int_through_var - int)

    }
}
