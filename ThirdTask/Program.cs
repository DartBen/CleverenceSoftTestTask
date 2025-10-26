using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ThirdTask.Service;

namespace ThirdTask
{
    internal class Program
    {

        // Регулярные выражения для каждого формата
        private static readonly Regex Format1Regex = new Regex(
            @"^(?<date>\d{2}\.\d{2}\.\d{4})\s+(?<time>\d{2}:\d{2}:\d{2}\.\d{3})\s+(?<loglevel>INFORMATION|WARNING|ERROR|DEBUG)(?<invoker>)\s+(?<message>.*)$",
            RegexOptions.Compiled);

        private static readonly Regex Format2Regex = new Regex(
            @"^(?<date>\d{4}-\d{2}-\d{2})\s+(?<time>\d{2}:\d{2}:\d{2}\.\d{4})\|\s+(?<loglevel>INFO|WARN|ERROR|DEBUG)\|(\d+)\|(?<invoker>[^|]*)\|\s+(?<message>.*)$",
            RegexOptions.Compiled);

        static List<IPatternLogChecker> parsers = new List<IPatternLogChecker>();

        static void Main(string[] args)
        {
            string inputFile = "input.log";
            string outputFile = "output.log";
            string problemFile = "problems.txt";

            var formatParser = new LogParser(Format1Regex);

            parsers.Add(formatParser);
            parsers.Add(new LogParser(Format2Regex));

            using var reader = new StreamReader(inputFile);
            using var writer = new StreamWriter(outputFile, append: false);
            using var problemWriter = new StreamWriter(problemFile, append: false);

            string line;
            int lineNumber = 0;
            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                var result = ParseLogLine(line);

                if (result.IsSuccess)
                {
                    writer.WriteLine($"{result.Date} {result.Time} {result.LogLevel} {result.InvokerMethod} {result.Message}");
                }
                else
                {
                    // Записываем исходную строку в файл проблем
                    problemWriter.WriteLine(line);
                }
            }

            Console.WriteLine("Обработка логов завершена. Результаты в output.log, проблемы в problems.txt");


            Console.ReadKey();
            GC.Collect();
            Console.ReadKey();

        }

        private static LogMetadata ParseLogLine(string logLine)
        {
            LogMetadata badResult;
            try
            {
                foreach (LogParser parser in parsers)
                {

                    var parseResult = parser.TryCheckString(logLine);

                    if (parseResult.IsSuccess)
                    {
                        return parseResult;
                    }
                }
            }
            catch (Exception e)
            {
                return (LogMetadata.BadResult(false, logLine));
            }
            return (LogMetadata.BadResult(false, logLine));
        }
    }
}
