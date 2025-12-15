using Lab12;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;

namespace Lab12
{
    public class KDALog
    {
        private const string LogFileName = "kdalogfile.txt";
        private static string LogPath = Path.Combine(Environment.CurrentDirectory, LogFileName);

        public void WriteLog(string action, string details)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(LogPath, true))
                {
                    sw.WriteLine($"{DateTime.Now:dd.MM.yyyy HH:mm:ss} | Действие: {action} | Инфо: {details}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка лога: {ex.Message}");
            }
        }

        public void ReadLog()
        {
            if (!File.Exists(LogPath)) return;
            using (StreamReader sr = new StreamReader(LogPath))
            {
                Console.WriteLine(sr.ReadToEnd());
            }
        }

        public void SearchLog(string keyword)
        {
            var lines = File.ReadAllLines(LogPath);
            var results = lines.Where(l => l.Contains(keyword));
            foreach (var line in results) Console.WriteLine(line);
        }

        public int CountRecords() => File.Exists(LogPath) ? File.ReadAllLines(LogPath).Length : 0;

        public void CleanOldRecords()
        {
            if (!File.Exists(LogPath)) return;
            var lines = File.ReadAllLines(LogPath);
            var currentHour = DateTime.Now.Hour;
            var currentDay = DateTime.Now.Date;

            var filtered = lines.Where(line =>
            {
                if (DateTime.TryParse(line.Split('|')[0].Trim(), out DateTime dt))
                {
                    return dt.Date == currentDay && dt.Hour == currentHour;
                }
                return false;
            }).ToList();

            File.WriteAllLines(LogPath, filtered);
        }
    }
}

public class KDADiskInfo
{
    private KDALog _log;
    public KDADiskInfo(KDALog log) => _log = log;

    public void ShowDiskDetails()
    {
        _log.WriteLog("DiskInfo", "Запрос информации о дисках");
        foreach (var drive in DriveInfo.GetDrives())
        {
            if (drive.IsReady)
            {
                Console.WriteLine($"Имя: {drive.Name}, Свободно: {drive.AvailableFreeSpace / 1024 / 1024} MB");
                Console.WriteLine($"FS: {drive.DriveFormat}, Объем: {drive.TotalSize / 1024 / 1024} MB, Метка: {drive.VolumeLabel}");
            }
        }
    }
}

// 3. KDAFileInfo
public class KDAFileInfo
{
    private KDALog _log;
    public KDAFileInfo(KDALog log) => _log = log;

    public void ShowFileInfo(string path)
    {
        _log.WriteLog("FileInfo", $"Запрос информации о файле: {path}");
        FileInfo fi = new FileInfo(path);
        if (fi.Exists)
        {
            Console.WriteLine($"Путь: {fi.FullName}\nРазмер: {fi.Length} B\nРасширение: {fi.Extension}");
            Console.WriteLine($"Создан: {fi.CreationTime}, Изменен: {fi.LastWriteTime}");
        }
    }
}

public class KDADirInfo
{
    private KDALog _log;
    public KDADirInfo(KDALog log) => _log = log;

    public void ShowDirInfo(string path)
    {
        _log.WriteLog("DirInfo", $"Запрос информации о папке: {path}");
        DirectoryInfo di = new DirectoryInfo(path);
        if (di.Exists)
        {
            Console.WriteLine($"Файлов: {di.GetFiles().Length}");
            Console.WriteLine($"Подпапок: {di.GetDirectories().Length}");
            Console.WriteLine($"Родитель: {di.Parent?.Name ?? "Нет"}");
        }
    }
}


public class KDAManager
{
    private KDALog _log;
    public KDAManager(KDALog log) => _log = log;

    public void ManageFiles(string driveName)
    {
        try
        {
            string inspectPath = Path.Combine(Environment.CurrentDirectory, "KDAInspect");
            if (Directory.Exists(inspectPath)) Directory.Delete(inspectPath, true);
            Directory.CreateDirectory(inspectPath);

            string infoFile = Path.Combine(inspectPath, "kdadirinfo.txt");
            using (StreamWriter sw = new StreamWriter(infoFile))
            {
                sw.WriteLine("Список папок и файлов диска " + driveName);
                foreach (var d in Directory.GetDirectories(driveName)) sw.WriteLine("[DIR] " + d);
                foreach (var f in Directory.GetFiles(driveName)) sw.WriteLine("[FILE] " + f);
            }

            string copyPath = Path.Combine(inspectPath, "kdadirinfo_copy.txt");
            File.Copy(infoFile, copyPath, true);
            File.Delete(infoFile);
            _log.WriteLog("FileManager", "Создана структура KDAInspect");

            string filesDir = Path.Combine(Environment.CurrentDirectory, "KDAFiles");
            if (Directory.Exists(filesDir)) Directory.Delete(filesDir, true);
            Directory.CreateDirectory(filesDir);

            foreach (var file in Directory.GetFiles(Environment.CurrentDirectory, "*.txt"))
            {
                File.Copy(file, Path.Combine(filesDir, Path.GetFileName(file)), true);
            }

            string movedFilesDir = Path.Combine(inspectPath, "KDAFiles");
            if (Directory.Exists(movedFilesDir)) Directory.Delete(movedFilesDir, true);
            Directory.Move(filesDir, movedFilesDir);
            _log.WriteLog("FileManager", "Файлы перемещены в KDAInspect");

            string zipPath = Path.Combine(Environment.CurrentDirectory, "archive.zip");
            if (File.Exists(zipPath)) File.Delete(zipPath);
            ZipFile.CreateFromDirectory(movedFilesDir, zipPath);

            string extractPath = Path.Combine(Environment.CurrentDirectory, "Extracted");
            if (Directory.Exists(extractPath)) Directory.Delete(extractPath, true);
            ZipFile.ExtractToDirectory(zipPath, extractPath);
            _log.WriteLog("FileManager", "Архивация выполнена");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка менеджера: {ex.Message}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        KDALog logger = new KDALog();

        try
        {
            KDADiskInfo diskInfo = new KDADiskInfo(logger);
            diskInfo.ShowDiskDetails();

            KDAFileInfo fileInfo = new KDAFileInfo(logger);
            fileInfo.ShowFileInfo("kdalogfile.txt");

            KDADirInfo dirInfo = new KDADirInfo(logger);
            dirInfo.ShowDirInfo(Environment.CurrentDirectory);

            KDAManager manager = new KDAManager(logger);
            manager.ManageFiles(@"C:\"); 

            Console.WriteLine("\n--- Поиск в логе по слову 'DiskInfo' ---");
            logger.SearchLog("DiskInfo");

            Console.WriteLine($"Всего записей: {logger.CountRecords()}");

            logger.CleanOldRecords();
            Console.WriteLine("Лог очищен. Оставлены только записи за текущий час.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка: {ex.Message}");
        }

        Console.WriteLine("Нажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}