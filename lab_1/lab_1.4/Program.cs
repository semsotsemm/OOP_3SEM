using System;

class Program
{
    static void Main()
    {
        //4.a Кортеж из 5 элементов
        (int, string, char, string, ulong) tuple = (42, "Hello", 'A', "World", 9999999999);

        //4.b Вывод кортежа
        Console.WriteLine("Кортеж целиком:");
        Console.WriteLine(tuple);

        Console.WriteLine("\nВыборочные элементы:");
        Console.WriteLine($"1 элемент = {tuple.Item1}");
        Console.WriteLine($"3 элемент = {tuple.Item3}");
        Console.WriteLine($"4 элемент = {tuple.Item4}");

        //4.c Распаковка кортежа
        Console.WriteLine("\nРаспаковка кортежа:");

        // 1 способ: полная распаковка
        (int num, string str1, char symbol, string str2, ulong bigNum) = tuple;
        Console.WriteLine($"num = {num}, str1 = {str1}, symbol = {symbol}, str2 = {str2}, bigNum = {bigNum}");

        // 2 способ: частичная распаковка с использованием '_'
        (int num2, _, char sym2, _, ulong bigNum2) = tuple;
        Console.WriteLine($"num2 = {num2}, sym2 = {sym2}, bigNum2 = {bigNum2}");

        // 3 способ: распаковка в существующие переменные
        int x; string s; char c; string s2; ulong u;
        (x, s, c, s2, u) = tuple;
        Console.WriteLine($"x = {x}, s = {s}, c = {c}, s2 = {s2}, u = {u}");

        //4.d) Сравнение двух кортежей
        var tuple1 = (1, "test", 'Q');
        var tuple2 = (1, "test", 'Q');
        var tuple3 = (2, "other", 'W');

        Console.WriteLine("\nСравнение кортежей:");
        Console.WriteLine($"tuple1 == tuple2 ? {tuple1 == tuple2}");
        Console.WriteLine($"tuple1 == tuple3 ? {tuple1 == tuple3}");
    }
}
