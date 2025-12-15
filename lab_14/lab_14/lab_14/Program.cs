using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace LabOS
{
    class Program
    {
        private static readonly object lockFile = new object();
        private static readonly object lockConsole = new object();
        private static readonly object lockSequential = new object();
        private static bool isEvenTurn = true;

        private static Thread primeThread;
        private static CancellationTokenSource cts;
        private static readonly object primeLock = new object();
        private static bool isPaused = false;

        private static int timerCount = 0;
        private static System.Threading.Timer consoleTimer;

        private static readonly SemaphoreSlim warehouseSemaphore = new SemaphoreSlim(1, 1);
        private const string InventoryFile = "inventory.txt";
        private static int totalItems = 0;

        private static readonly SemaphoreSlim channelSemaphore = new SemaphoreSlim(3); 
        private const int MaxWaitTimeMs = 2000;

        static void Main(string[] args)
        {
            Console.WriteLine("--- Лабораторная работа по ОС и многопоточности в C# ---");

            Task1_Processes();

            Task2_AppDomains();

            Task3_PrimeThread();

            Task4_EvenOddThreads();

            Task5_Timer();

            Task_Extra1_Warehouse();

            Task_Extra2_VideoChannels();

            Console.WriteLine("\n--- Работа завершена. Нажмите любую клавишу для выхода ---");
            Console.ReadKey();

            consoleTimer?.Dispose();
        }

        #region Задача 1: Процессы
        static void Task1_Processes()
        {
            Console.WriteLine("\n\n--- Задача 1: Запущенные процессы ---");

            try
            {
                string fileName = "processes_info.txt";
                using (StreamWriter sw = new StreamWriter(fileName, false))
                {
                    var processes = Process.GetProcesses().OrderBy(p => p.ProcessName).ToList();
                    Console.WriteLine($"Всего процессов: {processes.Count}. Информация также записана в файл: {fileName}");
                    sw.WriteLine($"Время генерации: {DateTime.Now}\n");

                    foreach (var process in processes)
                    {
                        try
                        {
                            string info = $"ID: {process.Id}, Имя: {process.ProcessName}, Приоритет: {process.BasePriority}";

                            try
                            {
                                info += $", Запуск: {process.StartTime}, ЦПУ (общее): {process.TotalProcessorTime.TotalSeconds:F2}s";
                            }
                            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception || ex is InvalidOperationException)
                            {
                                info += ", (Время/ЦПУ: Доступ запрещен)";
                            }

                            info += $", Состояние: {(process.Responding ? "Отвечает" : "Не отвечает")}";

                            Console.WriteLine(info);
                            sw.WriteLine(info);
                        }
                        catch (Exception ex)
                        {
                            string errorInfo = $"Ошибка при чтении процесса {process.Id} ({process.ProcessName}): {ex.Message}";
                            Console.WriteLine(errorInfo);
                            sw.WriteLine(errorInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка в Задаче 1: {ex.Message}");
            }
        }
        #endregion

        #region Задача 2: Домены приложений
        static void Task2_AppDomains()
        {
            Console.WriteLine("\n\n--- Задача 2: Домены приложений (AssemblyLoadContext) ---");

            AppDomain currentDomain = AppDomain.CurrentDomain;
            Console.WriteLine($"\n**Текущий домен (.NET Core AppContext):**");
            Console.WriteLine($"Имя: {currentDomain.FriendlyName}");
            Console.WriteLine($"Конфигурация (BaseDirectory): {AppContext.BaseDirectory}");
            Console.WriteLine($"Сборки:");
            foreach (var assembly in currentDomain.GetAssemblies().OrderBy(a => a.GetName().Name).Take(5)) 
            {
                Console.WriteLine($"- {assembly.GetName().Name} (Версия: {assembly.GetName().Version})");
            }
            Console.WriteLine($"(... всего {currentDomain.GetAssemblies().Length} сборок)");


            Console.WriteLine("\n**Создание и выгрузка нового домена:**");

            var alc = new CustomAssemblyLoadContext("TestLoadContext", true);
            Console.WriteLine($"Создан новый контекст: {alc.Name}");

            try
            {
                var assemblyPath = typeof(System.Collections.Generic.List<>).Assembly.Location;
                var loadedAssembly = alc.LoadFromAssemblyPath(assemblyPath);
                Console.WriteLine($"Загружена сборка в контекст: {loadedAssembly.GetName().Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось загрузить сборку в контекст: {ex.Message}. Продолжаем...");
            }

            alc.Unload();
            Console.WriteLine($"Контекст '{alc.Name}' выгружен.");

            for (int i = 0; alc.IsCollectible && alc.Assemblies.Any() && i < 10; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(100);
            }
        }

        class CustomAssemblyLoadContext : AssemblyLoadContext
        {
            public CustomAssemblyLoadContext(string name, bool isCollectible) : base(name, isCollectible) { }
        }
        #endregion

        #region Задача 3: Поток простых чисел
        static void Task3_PrimeThread()
        {
            Console.WriteLine("\n\n--- Задача 3: Управление потоком простых чисел ---");
            Console.Write("Введите N для поиска простых чисел от 1 до N: ");
            if (!int.TryParse(Console.ReadLine(), out int n) || n < 1)
            {
                Console.WriteLine("Некорректный ввод. Используем N=100.");
                n = 100;
            }

            cts = new CancellationTokenSource();
            primeThread = new Thread(() => FindPrimes(n, cts.Token))
            {
                Name = "PrimeFinderThread",
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };

            Console.WriteLine("Нажмите S для старта, P для паузы, R для возобновления, T для завершения.");

            primeThread.Start();
            Console.WriteLine($"[Main] Поток '{primeThread.Name}' запущен.");
            DisplayThreadInfo(primeThread, "После старта");

            while (primeThread.IsAlive)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    lock (primeLock)
                    {
                        switch (key)
                        {
                            case ConsoleKey.P:
                                if (!isPaused)
                                {
                                    isPaused = true;
                                    Console.WriteLine($"\n[Main] Поток '{primeThread.Name}' приостановлен.");
                                }
                                else
                                {
                                    Console.WriteLine($"\n[Main] Поток '{primeThread.Name}' уже приостановлен.");
                                }
                                break;
                            case ConsoleKey.R:
                                if (isPaused)
                                {
                                    isPaused = false;
                                    Monitor.Pulse(primeLock); 
                                    Console.WriteLine($"\n[Main] Поток '{primeThread.Name}' возобновлен.");
                                }
                                else
                                {
                                    Console.WriteLine($"\n[Main] Поток '{primeThread.Name}' уже выполняется.");
                                }
                                break;
                            case ConsoleKey.T:
                                cts.Cancel(); 
                                Console.WriteLine($"\n[Main] Запрос на завершение потока '{primeThread.Name}'.");
                                break;
                        }
                    }
                }

                DisplayThreadInfo(primeThread, "Во время выполнения");
                Thread.Sleep(500);
            }

            Console.WriteLine($"[Main] Поток '{primeThread.Name}' завершил работу.");
        }

        static void FindPrimes(int n, CancellationToken token)
        {
            string fileName = "primes.txt";
            File.WriteAllText(fileName, $"Простые числа от 1 до {n}:\n");

            for (int i = 2; i <= n; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine($"\n[PrimeThread] Поток остановлен по запросу.");
                    return;
                }

                lock (primeLock)
                {
                    while (isPaused)
                    {
                        Monitor.Wait(primeLock);
                    }
                }

                Thread.Sleep(1); 
                if (IsPrime(i))
                {
                    Console.Write($"{i} ");
                    File.AppendAllText(fileName, $"{i}\n");
                }
            }
            Console.WriteLine($"\n[PrimeThread] Расчет завершен.");
        }

        static bool IsPrime(int number)
        {
            if (number < 2) return false;
            for (int i = 2; i * i <= number; i++)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        static void DisplayThreadInfo(Thread thread, string phase)
        {
            Console.WriteLine($"\n[{phase}] ID: {thread.ManagedThreadId}, Имя: {thread.Name}, Приоритет: {thread.Priority}, Статус: {thread.ThreadState}, Alive: {thread.IsAlive}, Paused: {isPaused}");
        }
        #endregion

        #region Задача 4: Четные/Нечетные потоки с синхронизацией
        static void Task4_EvenOddThreads()
        {
            Console.WriteLine("\n\n--- Задача 4: Четные/Нечетные потоки ---");
            Console.Write("Введите N для четных/нечетных чисел: ");
            if (!int.TryParse(Console.ReadLine(), out int n) || n < 1)
            {
                Console.WriteLine("Некорректный ввод. Используем N=20.");
                n = 20;
            }

            string commonFile = "even_odd.txt";
            File.WriteAllText(commonFile, $"Четные/Нечетные числа до {n}:\n");

            Thread evenThread = new Thread(() => EvenWorker(n, commonFile, 200)) { Name = "EvenThread", Priority = ThreadPriority.Normal };
            Thread oddThread = new Thread(() => OddWorker(n, commonFile, 100)) { Name = "OddThread", Priority = ThreadPriority.Normal };

            Console.WriteLine("\n**4. Без синхронизации (разная скорость):**");
            evenThread.Start();
            oddThread.Start();
            evenThread.Join();
            oddThread.Join();
            Console.WriteLine("\n[Main] Асинхронный вывод завершен.");

            Console.WriteLine("\n**4a. Изменение приоритета (OddThread на Lowest):**");
            oddThread = new Thread(() => OddWorker(n, commonFile, 100)) { Name = "OddThread_Low", Priority = ThreadPriority.Lowest };
            evenThread = new Thread(() => EvenWorker(n, commonFile, 200)) { Name = "EvenThread_Normal", Priority = ThreadPriority.Normal };
            evenThread.Start();
            oddThread.Start();
            evenThread.Join();
            oddThread.Join();
            Console.WriteLine("\n[Main] Измененный приоритет завершен.");

            Console.WriteLine("\n**4b.i. Сначала четные, потом нечетные (синхронизация Join):**");
            evenThread = new Thread(() => EvenWorker(n, commonFile, 50, true)) { Name = "EvenThread_Join" };
            oddThread = new Thread(() => OddWorker(n, commonFile, 50, true)) { Name = "OddThread_Join" };

            evenThread.Start();
            evenThread.Join(); 
            oddThread.Start();
            oddThread.Join();  
            Console.WriteLine("\n[Main] Последовательный вывод (Join) завершен.");

            Console.WriteLine("\n**4b.ii. Последовательный вывод (Monitor):**");
            isEvenTurn = true; 
            Thread evenSequential = new Thread(() => SequentialWorker(n, commonFile, true)) { Name = "EvenSequential" };
            Thread oddSequential = new Thread(() => SequentialWorker(n, commonFile, false)) { Name = "OddSequential" };

            evenSequential.Start();
            oddSequential.Start();

            evenSequential.Join();
            oddSequential.Join();
            Console.WriteLine("\n[Main] Поочередный вывод (Monitor) завершен.");
        }

        static void EvenWorker(int n, string file, int delayMs, bool joinSync = false)
        {
            if (joinSync)
            {
                Console.WriteLine($"[{Thread.CurrentThread.Name}] - Начало.");
            }

            for (int i = 2; i <= n; i += 2)
            {
                lock (lockConsole)
                {
                    Console.Write($"E{i} ");
                }
                lock (lockFile)
                {
                    File.AppendAllText(file, $"E{i}\n");
                }
                Thread.Sleep(delayMs);
            }

            if (joinSync)
            {
                Console.WriteLine($"\n[{Thread.CurrentThread.Name}] - Завершено.");
            }
        }

        static void OddWorker(int n, string file, int delayMs, bool joinSync = false)
        {
            if (joinSync)
            {
                Console.WriteLine($"[{Thread.CurrentThread.Name}] - Начало.");
            }

            for (int i = 1; i <= n; i += 2)
            {
                lock (lockConsole)
                {
                    Console.Write($"O{i} ");
                }
                lock (lockFile)
                {
                    File.AppendAllText(file, $"O{i}\n");
                }
                Thread.Sleep(delayMs);
            }

            if (joinSync)
            {
                Console.WriteLine($"\n[{Thread.CurrentThread.Name}] - Завершено.");
            }
        }

        static void SequentialWorker(int n, string file, bool isEven)
        {
            int start = isEven ? 2 : 1;

            for (int i = start; i <= n; i += 2)
            {
                lock (lockSequential)
                {
                    while (isEven != isEvenTurn)
                    {
                        Monitor.Wait(lockSequential);
                    }

                    char prefix = isEven ? 'E' : 'O';
                    Console.Write($"{prefix}{i} ");
                    File.AppendAllText(file, $"{prefix}{i}\n");

                    isEvenTurn = !isEvenTurn;
                    Monitor.Pulse(lockSequential);
                }
                Thread.Sleep(50); 
            }
        }
        #endregion

        #region Задача 5: Повторяющаяся задача на основе класса Timer
        static void Task5_Timer()
        {
            Console.WriteLine("\n\n--- Задача 5: Класс System.Threading.Timer ---");
            Console.WriteLine("Таймер запущен. Он будет выводить информацию каждые 2 секунды в течение 10 секунд.");

            consoleTimer = new System.Threading.Timer(TimerCallback, null, 0, 2000);

            Thread.Sleep(10000);

            consoleTimer.Dispose();
            Console.WriteLine("\n[Main] Таймер остановлен (Dispose вызван).");
        }

        static void TimerCallback(object state)
        {
            timerCount++;
            Console.WriteLine($"\n[{DateTime.Now.ToLongTimeString()}] [Timer] Вызов #{timerCount}. Поток ID: {Thread.CurrentThread.ManagedThreadId}");

            if (timerCount >= 5)
            {
            }
        }
        #endregion

        #region Дополнительное задание 1: Склад (Семафор 1)
        static void Task_Extra1_Warehouse()
        {
            Console.WriteLine("\n\n--- Дополнительное задание 1: Склад (Семафор 1) ---");

            totalItems = 50;
            File.WriteAllText(InventoryFile, $"Остаток на складе: {totalItems}\n");
            Console.WriteLine($"На складе изначально: {totalItems} товаров.");

            Thread machineA = new Thread(() => MachineWorker("A", 100)) { Name = "Machine A" };
            Thread machineB = new Thread(() => MachineWorker("B", 200)) { Name = "Machine B" };
            Thread machineC = new Thread(() => MachineWorker("C", 300)) { Name = "Machine C" };

            machineA.Start();
            machineB.Start();
            machineC.Start();

            machineA.Join();
            machineB.Join();
            machineC.Join();

            Console.WriteLine($"\n[Main] Разгрузка склада завершена. Итоговый остаток: {totalItems}");
        }

        static void MachineWorker(string name, int speedMs)
        {
            while (true)
            {
                Console.WriteLine($"[{name}] Ожидает доступ к складу...");
                warehouseSemaphore.Wait(); 

                try
                {
                    if (totalItems <= 0)
                    {
                        Console.WriteLine($"[{name}] Склад пуст. Завершение работы.");
                        return;
                    }

                    int loadAmount = Math.Min(5, totalItems);
                    totalItems -= loadAmount;

                    Console.WriteLine($"[{name}] Загружает {loadAmount} ед. (скорость {speedMs}ms). Остаток: {totalItems}");
                    File.AppendAllText(InventoryFile, $"{name}: Загружено {loadAmount}. Остаток: {totalItems}\n");

                    Thread.Sleep(speedMs * loadAmount / 5);
                }
                finally
                {
                    warehouseSemaphore.Release(); 
                    Console.WriteLine($"[{name}] Освободил доступ к складу.");
                }

                Thread.Sleep(500);
            }
        }
        #endregion

        #region Дополнительное задание 2: Видеоканалы (Семафор N)
        static void Task_Extra2_VideoChannels()
        {
            Console.WriteLine("\n\n--- Дополнительное задание 2: Видеоканалы (Семафор N) ---");
            Console.WriteLine($"Всего доступно каналов: {channelSemaphore.CurrentCount}");

            Task[] clientTasks = new Task[7];
            for (int i = 1; i <= 7; i++)
            {
                int clientId = i;
                clientTasks[i - 1] = Task.Run(() => ClientWorker(clientId));
            }

            Task.WaitAll(clientTasks);
            Console.WriteLine("[Main] Все клиенты завершили работу.");
        }

        static void ClientWorker(int id)
        {
            Console.WriteLine($"[Client {id}] Хочет получить доступ к каналу.");

            if (channelSemaphore.Wait(MaxWaitTimeMs))
            {
                try
                {
                    Console.WriteLine($"[Client {id}] ✅ Получил доступ к каналу. Занято каналов: {3 - channelSemaphore.CurrentCount}");

                    Thread.Sleep(new Random().Next(1000, 3000));

                    Console.WriteLine($"[Client {id}] Закончил использование канала.");
                }
                finally
                {
                    channelSemaphore.Release(); 
                    Console.WriteLine($"[Client {id}] Освободил канал. Свободно каналов: {channelSemaphore.CurrentCount}");
                }
            }
            else
            {
                Console.WriteLine($"[Client {id}] ❌ Не смог получить канал за {MaxWaitTimeMs / 1000}с и ушел.");
            }
        }
        #endregion
    }
}