using System;

namespace lab_1._3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //3.a
            int[][] matrix = new int[3][];
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = new int[3];
            }
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix.Length; j++)
                {
                    matrix[i][j] = i + j;
                }
            }
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix.Length; j++)
                {
                    Console.Write(matrix[i][j]);
                }
                Console.WriteLine();
            }
            //3.b
            string[] line_array = { "First", "progrma", "on", "c#" };
            Console.Write("Элементы массива:\n");
            for (int i = 0; i < line_array.Length; i++)
            {
                Console.WriteLine(line_array[i]);
            }
            Console.WriteLine($"Длина массива: {line_array.Length}");
            Console.WriteLine("Введите позицию элемента, который хотите изменить (номера позиций начинаются с 0): ");
            int index = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine($"Cтарое значение элемента: {line_array[index]}, введите новое значение:");
            string new_element = Console.ReadLine();
            line_array[index] = new_element;
            Console.Write("Элементы массива, после изменения:\n");
            for (int i = 0; i < line_array.Length; i++)
            {
                Console.WriteLine(line_array[i]);
            }
            //3.c
            double[][] arr = new double[3][];

            arr[0] = new double[2]; 
            arr[1] = new double[3];
            arr[2] = new double[4]; 

            Console.WriteLine("Введите элементы массива:");

            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr[i].Length; j++)
                {
                    Console.Write($"arr[{i}][{j}] = ");
                    arr[i][j] = double.Parse(Console.ReadLine());
                }
            }

            Console.WriteLine("\nВы ввели массив:");
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr[i].Length; j++)
                {
                    Console.Write(arr[i][j] + "\t");
                }
                Console.WriteLine();
            }
            //3.d
            var numbers = new int[] { 1, 2, 3, 4, 5 };
            var text = "Привет, мир!";
            Console.WriteLine("Массив:");
            for (int i = 0; i < numbers.Length; i++)
            {
                Console.Write(numbers[i] + " ");
            }

            Console.WriteLine("\nСтрока:");
            for (int i = 0; i < text.Length; i++)
            {
                Console.Write(text[i]);
            }
        }
    }
}
