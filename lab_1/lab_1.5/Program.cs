using System;

class Program
{
    static void Main()
    {
        int[] numbers = { 5, 8, -2, 10, 3 };
        string text = "Привет";

        (int max, int min, int sum, char firstLetter) AnalyzeArray(int[] arr, string str)
        {
            int max = arr[0];
            int min = arr[0];
            int sum = 0;

            foreach (var num in arr)
            {
                if (num > max) max = num;
                if (num < min) min = num;
                sum += num;
            }

            char firstLetter = str.Length > 0 ? str[0] : '\0';

            return (max, min, sum, firstLetter);
        }

        var result = AnalyzeArray(numbers, text);

        Console.WriteLine($"Максимум: {result.max}");
        Console.WriteLine($"Минимум: {result.min}");
        Console.WriteLine($"Сумма: {result.sum}");
        Console.WriteLine($"Первая буква строки: {result.firstLetter}");
    }
}
