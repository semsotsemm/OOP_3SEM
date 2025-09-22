using System;

class Program
{
    static void Main()
    {
        void CheckedExample()
        {
            Console.WriteLine("=== Блок checked ===");
            try
            {
                checked
                {
                    int max = int.MaxValue;
                    Console.WriteLine($"Начальное значение: {max}");
                    int overflow = max + 1;
                    Console.WriteLine($"После переполнения: {overflow}");
                }
            }
            catch (OverflowException ex)
            {
                Console.WriteLine($"Исключение: {ex.Message}");
            }
        }

        void UncheckedExample()
        {
            Console.WriteLine("\n=== Блок unchecked ===");
            unchecked
            {
                int max = int.MaxValue;
                Console.WriteLine($"Начальное значение: {max}");
                int overflow = max + 1;
                Console.WriteLine($"После переполнения: {overflow}");
            }
        }

        CheckedExample();
        UncheckedExample();
    }
}
