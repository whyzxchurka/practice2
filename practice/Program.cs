using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.IO;

namespace practice
{
    internal class Program
    {
        const string InputFile = "input.json";
        const string OutputFile = "output.json";

        static void Main()
        {
            Console.WriteLine("Калькулятор статистики");
            Console.WriteLine("Введите числа по одному. Пустая строка (enter) закрывает программу\n");

            var numbers = ReadNumbers();

            if (numbers.Count == 0)
            {
                Console.WriteLine("Не введено чисел. Выход");
                return;
            }

            WriteInputFile(numbers);

            bool ok = RunPython();
            if (!ok)
            {
                Console.WriteLine("Python завершился с ошибкой");
                return;
            }

            ShowResults();
        }

        static List<double> ReadNumbers()
        {
            var list = new List<double>();

            while (true)
            {
                Console.Write($"Число {list.Count + 1}: ");
                string line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                    break;

                if (double.TryParse(line, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double num))
                {
                    list.Add(num);
                }
                else
                {
                    Console.WriteLine("Не является числом, пропуск");
                }
            }

            return list;
        }

        static void WriteInputFile(List<double> numbers)
        {
            // добавляем числа в файл json 
            var payload = new { numbers };
            string json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(InputFile, json);
        }

        static bool RunPython()
        {
            // запуск python процессом
            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = "analyzer.py",
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using (var proc = Process.Start(psi))
            {
                if (proc == null)
                {
                    Console.WriteLine("Не удалось запустить Python");
                    return false;
                }

                string stderr = proc.StandardError.ReadToEnd();
                proc.WaitForExit();

                if (proc.ExitCode != 0)
                {
                    Console.WriteLine("Ошибка в Python: " + stderr);
                    return false;
                }
            }
            return true;
        }

        static void ShowResults()
        {
            if (!File.Exists(OutputFile))
            {
                Console.WriteLine("Файл с результатами не найден");
                return;
            }

            string json = File.ReadAllText(OutputFile);
            using (var doc = JsonDocument.Parse(json))
            {
                var root = doc.RootElement;

                Console.WriteLine("\nРезультат");
                Console.WriteLine($"Количество: {root.GetProperty("count").GetInt32()}");
                Console.WriteLine($"Сумма: {root.GetProperty("sum").GetDouble()}");
                Console.WriteLine($"Среднее: {root.GetProperty("average").GetDouble():F4}");
                Console.WriteLine($"Минимум: {root.GetProperty("min").GetDouble()}");
                Console.WriteLine($"Максимум: {root.GetProperty("max").GetDouble()}");
            }
        }
    }
}
