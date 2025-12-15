using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class TplLab
{
    private const int N = 2_000_000; 
    private const int ARRAY_SIZE = 1_000_000;

    private static int SieveOfEratosthenes(int max)
    {
        bool[] isPrime = new bool[max + 1];
        for (int i = 2; i <= max; i++)
        {
            isPrime[i] = true;
        }

        for (int p = 2; p * p <= max; p++)
        {
            if (isPrime[p])
            {
                for (int i = p * p; i <= max; i += p)
                {
                    isPrime[i] = false;
                }
            }
        }

        return isPrime.Count(p => p);
    }

    private static void Task1_Run()
    {
        Console.WriteLine("\n--- Задание 1: Длительная задача (Решето Эратосфена) ---");

        Task<int> sieveTask = Task.Run(() =>
        {
            Console.WriteLine($"\n[Task ID: {Task.CurrentId}] Задача запущена.");

            Console.WriteLine($"[Task ID: {Task.CurrentId}] Текущий статус: Running");

            int count = SieveOfEratosthenes(N);
            return count;
        });

        Console.WriteLine($"\n[Main Thread] До ожидания: ID задачи: {sieveTask.Id}, Статус: {sieveTask.Status}");

        try
        {
            sieveTask.Wait(); 
            Console.WriteLine($"[Task ID: {sieveTask.Id}] Задача завершена. Найдено простых чисел: {sieveTask.Result}");
        }
        catch (AggregateException ex)
        {
            Console.WriteLine($"Произошла ошибка в задаче: {ex.InnerException.Message}");
        }

        Console.WriteLine($"[Main Thread] После ожидания: ID задачи: {sieveTask.Id}, Статус: {sieveTask.Status}\n");

        const int RUNS = 5;
        Console.WriteLine($"--- Оценка производительности ({RUNS} прогонов) ---");

        MeasurePerformance(Task1_Sequential, RUNS, "Последовательный");

        MeasurePerformance(Task1_Parallel, RUNS, "Параллельный (Task.Run)");
    }

    private static void Task1_Sequential()
    {
        SieveOfEratosthenes(N);
    }

    private static void Task1_Parallel()
    {
        Task.Run(() => SieveOfEratosthenes(N)).Wait();
    }

    private static void MeasurePerformance(Action action, int runs, string label)
    {
        Stopwatch sw = new Stopwatch();
        long totalMs = 0;

        for (int i = 0; i < runs; i++)
        {
            sw.Restart();
            action();
            sw.Stop();
            totalMs += sw.ElapsedMilliseconds;
        }

        Console.WriteLine($"- {label}: Среднее время выполнения: {totalMs / runs:F2} мс");
    }


    private static void Task2_Run()
    {
        Console.WriteLine("\n--- Задание 2: Задача с токеном отмены (CancellationToken) ---");
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        Task cancellationTask = Task.Run(() =>
        {
            try
            {
                Console.WriteLine($"[Task ID: {Task.CurrentId}] Задача с отменой запущена. Начнем отсчет до {N}.");
                for (int i = 0; i <= N; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine($"[Task ID: {Task.CurrentId}] Отмена запрошена. Прерывание...");
                        token.ThrowIfCancellationRequested(); 
                    }
                    if (i % (N / 4) == 0)
                    {
                        Console.WriteLine($"[Task ID: {Task.CurrentId}] Выполнено на {i * 100 / N}%...");
                        Thread.Sleep(5);
                    }
                }
                Console.WriteLine($"[Task ID: {Task.CurrentId}] Задача завершена без отмены.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"[Task ID: {Task.CurrentId}] Задача успешно отменена.");
            }
        }, token); 

        Task.Delay(100).ContinueWith(_ =>
        {
            if (cancellationTask.Status <= TaskStatus.Running) 
            {
                cts.Cancel();
                Console.WriteLine("[Main Thread] Запрос отмены отправлен.");
            }
        });

        try
        {
            cancellationTask.Wait();
        }
        catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
        {
            Console.WriteLine($"[Main Thread] Задача отмены: Итоговый статус: {cancellationTask.Status}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Main Thread] Произошла другая ошибка: {ex.Message}");
        }
    }

    private static int CalculateFactor(int baseValue, int delayMs)
    {
        Thread.Sleep(delayMs);
        return baseValue * 2 + new Random().Next(1, 10);
    }

    private static void Task3_Run()
    {
        Console.WriteLine("\n--- Задание 3: Задачи с возвратом результата (Task<TResult>) ---");

        Task<int> taskA = Task.Run(() => CalculateFactor(10, 100));
        Task<int> taskB = Task.Run(() => CalculateFactor(20, 200));
        Task<int> taskC = Task.Run(() => CalculateFactor(30, 50));

        Task<double> finalCalculation = Task.WhenAll(taskA, taskB, taskC).ContinueWith(antecedent =>
        {
            if (antecedent.Status == TaskStatus.Faulted)
            {
                throw new InvalidOperationException("Ошибка в одной из задач-предшественников.");
            }

            int resultA = taskA.Result;
            int resultB = taskB.Result;
            int resultC = taskC.Result;

            Console.WriteLine($"\n[Task 4] Результаты-предшественники: A={resultA}, B={resultB}, C={resultC}");

            double finalResult = (Math.Pow(resultA, 2) + resultB) / Math.Sqrt(resultC);

            return finalResult;
        });

        try
        {
            Console.WriteLine("[Main Thread] Ожидание финального расчета...");
            finalCalculation.Wait();
            Console.WriteLine($"[Main Thread] Финальный результат: {finalCalculation.Result:F2}");
        }
        catch (AggregateException ex)
        {
            Console.WriteLine($"[Main Thread] Ошибка при расчете: {ex.InnerException.Message}");
        }
    }


    private static void Task4_Run()
    {
        Console.WriteLine("\n--- Задание 4: Задача продолжения (Continuation Task) ---");
        Task<int> precursorTask = Task.Run(() =>
        {
            Thread.Sleep(100);
            return 42;
        });

        Task<string> continuationWith = precursorTask.ContinueWith(antecedent =>
        {
            if (antecedent.IsCompletedSuccessfully)
            {
                return $"[ContinueWith] Предшественник (ID: {antecedent.Id}) успешно завершен. Результат: {antecedent.Result}";
            }
            return "[ContinueWith] Предшественник не завершился успешно.";
        });

        var awaiter = precursorTask.GetAwaiter();
        string continuationAwaiterResult = "";

        awaiter.OnCompleted(() =>
        {
            continuationAwaiterResult = $"[GetAwaiter] Предшественник успешно завершен. Результат: {awaiter.GetResult()}";
        });

        Task.WaitAll(precursorTask, continuationWith);
        Console.WriteLine(continuationWith.Result);
        Console.WriteLine(continuationAwaiterResult);
    }


    private static void Task5_Run()
    {
        Console.WriteLine("\n--- Задание 5: Параллельные циклы (Parallel.For/ForEach) ---");

        int[] data = new int[ARRAY_SIZE];

        MeasureCyclePerformance(
            () =>
            {
                for (int i = 0; i < ARRAY_SIZE; i++)
                {
                    data[i] = i * 2 + 1; 
                }
            }, 3, "Обычный For");

        MeasureCyclePerformance(
            () =>
            {
                Parallel.For(0, ARRAY_SIZE, i =>
                {
                    data[i] = i * 2 + 1;
                });
            }, 3, "Parallel.For");

        List<string> strings = Enumerable.Range(0, 100000).Select(i => $"Item_{i}").ToList();

        MeasureCyclePerformance(
            () =>
            {
                foreach (var item in strings)
                {
                    string processed = item.Replace('_', '-').ToUpper();
                }
            }, 3, "Обычный ForEach");

        MeasureCyclePerformance(
            () =>
            {
                Parallel.ForEach(strings, item =>
                {
                    string processed = item.Replace('_', '-').ToUpper();
                });
            }, 3, "Parallel.ForEach");
    }

    private static void MeasureCyclePerformance(Action action, int runs, string label)
    {
        Stopwatch sw = new Stopwatch();
        long totalMs = 0;

        for (int i = 0; i < runs; i++)
        {
            sw.Restart();
            action();
            sw.Stop();
            totalMs += sw.ElapsedMilliseconds;
        }

        Console.WriteLine($"- {label}: Среднее время выполнения: {totalMs / runs:F2} мс");
    }


    private static void Task6_Run()
    {
        Console.WriteLine("\n--- Задание 6: Parallel.Invoke() ---");
        Stopwatch sw = Stopwatch.StartNew();

        Parallel.Invoke(
            () => DoWork("Задача A", 100),
            () => DoWork("Задача B", 250),
            () => DoWork("Задача C", 150)
        );

        sw.Stop();
        Console.WriteLine($"[Main Thread] Parallel.Invoke завершен. Общее время: {sw.ElapsedMilliseconds} мс");
        Console.WriteLine("(Это время должно быть близко к времени самой долгой задачи - 250 мс)");
    }

    private static void DoWork(string name, int durationMs)
    {
        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] {name} началась (длительность: {durationMs} мс)");
        Thread.Sleep(durationMs);
        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] {name} завершена.");
    }


    private static BlockingCollection<string> warehouse = new BlockingCollection<string>(new ConcurrentQueue<string>());
    private static int suppliesCount = 0;
    private static int customersServed = 0;
    private static object consoleLock = new object();

    private static void Task7_Run()
    {
        Console.WriteLine("\n--- Задание 7: Класс BlockingCollection (Поставщик-Потребитель) ---");

        int numSuppliers = 5;
        Task[] suppliers = new Task[numSuppliers];
        for (int i = 0; i < numSuppliers; i++)
        {
            suppliers[i] = Task.Run(() => Supplier(i + 1));
        }

        int numCustomers = 10;
        Task[] customers = new Task[numCustomers];
        for (int i = 0; i < numCustomers; i++)
        {
            customers[i] = Task.Run(() => Customer(i + 1));
        }

        Task.WaitAll(suppliers);

        warehouse.CompleteAdding();
        Console.WriteLine("\n[Склад] Все поставщики завершили работу.");

        Task.WaitAll(customers);

        Console.WriteLine($"[Итог] Всего завезено товаров: {suppliesCount}. Покупателей обслужено: {customersServed}. Остаток на складе: {warehouse.Count}.");
    }

    private static void Supplier(int id)
    {
        Random rand = new Random();
        for (int i = 1; i <= 3; i++)
        {
            string item = $"Товар_{id}-{i}";
            int delay = rand.Next(50, 400); 
            Thread.Sleep(delay);

            warehouse.Add(item); 
            Interlocked.Increment(ref suppliesCount);

            lock (consoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n[Поставщик {id}] Завез: {item}");
                DisplayWarehouseContent("после завоза");
            }
        }
    }

    private static void Customer(int id)
    {
        if (warehouse.IsAddingCompleted && warehouse.Count == 0)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[Покупатель {id}] Склад пуст и закрыт. Уходит.");
                Console.ResetColor();
            }
            return;
        }

        if (warehouse.TryTake(out string item))
        {
            Interlocked.Increment(ref customersServed);
            lock (consoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"\n[Покупатель {id}] Купил: {item}");
                DisplayWarehouseContent("после покупки");
            }
        }
        else
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Покупатель {id}] Товара нет. Уходит.");
                Console.ResetColor();
            }
        }
    }

    private static void DisplayWarehouseContent(string action)
    {
        Console.WriteLine($"[Склад] Состояние {action} ({warehouse.Count} шт.): {string.Join(", ", warehouse)}");
        Console.ResetColor();
    }

    private static async Task<string> SimulateLongOperationAsync(int durationMs)
    {
        Console.WriteLine($"\n[Task 8] Начало асинхронной операции в потоке: {Thread.CurrentThread.ManagedThreadId}");

        await Task.Delay(durationMs);

        Console.WriteLine($"[Task 8] Завершение асинхронной операции в потоке: {Thread.CurrentThread.ManagedThreadId}");
        return "Данные успешно получены после ожидания.";
    }

    private static async Task Task8_RunAsync()
    {
        Console.WriteLine("\n--- Задание 8: async и await ---");
        Console.WriteLine($"[Main Thread] Вызов асинхронного метода в потоке: {Thread.CurrentThread.ManagedThreadId}");

        Task<string> operationTask = SimulateLongOperationAsync(1500);

        Console.WriteLine("[Main Thread] Продолжение работы, не дожидаясь завершения операции...");

        string result = await operationTask;

        Console.WriteLine($"[Main Thread] Результат асинхронной операции: {result}");
    }


    public static async Task Main(string[] args)
    {
        Console.Title = "Лабораторная работа по TPL";
        Task1_Run();

        Task2_Run();

        Task3_Run();

        Task4_Run();

        Task5_Run();

        Task6_Run();

        Task7_Run();

        await Task8_RunAsync();

        Console.WriteLine("\n--- Все задания завершены. Нажмите любую клавишу для выхода ---");
        Console.ReadKey();
    }
}