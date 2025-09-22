using System;
using System.Text;

namespace lab_1._2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //2.a
            string line1 = "Hello";
            string line2 = "Alice";
            if (line1 == line2)
            {
                Console.WriteLine("Строки равны");
            }
            else
            {
                Console.WriteLine("Строки не равны");
            }
            //2.b

            Console.WriteLine("\n---------------------------------------------------------------------------------------------");
            string str1 = "First";
            string str2 = " program";
            string str3 = " on c#";

            // Сцепление
            string concatenated = str1 + str2 + str3;
            Console.WriteLine("Сцепление строк: " + concatenated);

            // Копирование
            string copy_str = string.Copy(str1);
            Console.WriteLine("Скопированная строка: {0}",copy_str);

            // Выделение подстроки
            string substring = str3.Substring(4, 2);
            Console.WriteLine("Подстрока: {0}", substring);

            // Разделение строки на слова
            string[] words = concatenated.Split(' ');
            Console.Write("Строка, разделенная на слова: ");
            for (int i = 0; i < words.Length; i++) {
                Console.Write(words[i] + ", ");
            }
            Console.WriteLine();

            // Вставить подстроку в определенную позицию
            Console.WriteLine("Вставить подстроку: " + concatenated.Insert(5, " вставленная подстрока"));

            // Удалить заданную подстроку
            Console.WriteLine("Удалить заданную подстроку: " + concatenated.Replace(" program", ""));

            // Интерполирование строк
            Console.WriteLine($"Интерполирование строк: {copy_str}, {substring}");

            //2.c

            Console.WriteLine("\n---------------------------------------------------------------------------------------------");
            string null_str = null;
            string empty_str = "";
            if (string.IsNullOrEmpty(null_str))
            {
                Console.WriteLine("Строка null_str = null или пустая");
            }
            else 
            {
                Console.WriteLine("Строка null_str не пустая");
            }
            if (string.IsNullOrEmpty(empty_str))
            {
                Console.WriteLine("Строка emptry_str = null или пустая");
            }
            else
            {
                Console.WriteLine("Строка emptry_str не пустая");
            }
            if (null_str == empty_str)
            {
                Console.WriteLine("Строки null_str и emptry_str равны");
            }
            else 
            {
                Console.WriteLine("Строки null_str и emptry_str не равны");
            }
            if (!string.IsNullOrEmpty(null_str))
            {
                Console.WriteLine($"Длина строки nullstr = {null_str.Length}");
            }
            else
            {
                Console.WriteLine($"Не удалось получить длину строки null_str");
            }
            if (!string.IsNullOrEmpty(empty_str))
            {
                Console.WriteLine(empty_str.Length);
            }
            else
            {
                Console.WriteLine($"Не удалось получить длину строки empty_str");
            }

            //2.d

            Console.WriteLine("\n---------------------------------------------------------------------------------------------");
            StringBuilder sb = new StringBuilder("Привет, мир!");
            Console.WriteLine("Исходная строка: " + sb);
            sb.Remove(6, 5);
            Console.WriteLine("После удаления: " + sb);
            sb.Insert(0, ">>> ");
            Console.WriteLine("После вставки в начало: " + sb);
            sb.Append(" <<<");
            Console.WriteLine("После вставки в конец: " + sb);
        }
    }
}
