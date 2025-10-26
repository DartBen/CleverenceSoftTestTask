using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace ThirdTask.Service
{
    public class LogProcessor : ILogProcessor
    {
        private readonly ILogger<LogProcessor> _logger;
        private readonly ILogParserFactory _parserFactory;

        private static readonly Regex Format1Regex =
            new Regex(@"^(?<date>\d{2}\.\d{2}\.\d{4})\s+(?<time>\d{2}:\d{2}:\d{2}\.\d{3})\s+(?<loglevel>INFORMATION|WARNING|ERROR|DEBUG)(?<invoker>)\s+(?<message>.*)$",
    RegexOptions.Compiled);

        private static readonly Regex Format2Regex = new Regex(@"^(?<date>\d{4}-\d{2}-\d{2})\s+(?<time>\d{2}:\d{2}:\d{2}\.\d{4})\|\s+(?<loglevel>INFO|WARN|ERROR|DEBUG)\|(\d+)\|(?<invoker>[^|]*)\|\s+(?<message>.*)$",
            RegexOptions.Compiled);

        LogProcessor(ILogParserFactory parserFactory, ILogger<LogProcessor> logger)
        {
            _logger = logger;
            _parserFactory = parserFactory;
        }

        public async Task ProcessAsync(string inputPath, string outputPath, string problemPath)
        {
            // Создаём парсеры через фабрику
            var parsers = new List<IPatternLogChecker>
            {
                _parserFactory.CreateParser(Format1Regex),
                _parserFactory.CreateParser(Format2Regex)
            };

            using var reader = new StreamReader(inputPath);
            using var writer = new StreamWriter(outputPath, append: false);
            using var problemWriter = new StreamWriter(problemPath, append: false);

            string line;
            int lineNumber = 0;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                lineNumber++;
                LogMetadata metadata = null;
                foreach (var parser in parsers)
                {
                    metadata = parser.TryCheckString(line);
                    if (metadata?.IsSuccess == true) break;
                }

                if (metadata != null && metadata.IsSuccess)
                {
                    // Т.к. ILogConverter удален, используем форматирование напрямую или создайте метод в LogMetadata
                    var convertedLine = $"{metadata.Date}\t{metadata.Time}\t{metadata.LogLevel}\t{metadata.InvokerMethod}\t{metadata.Message}";
                    await writer.WriteLineAsync(convertedLine);
                }
                else
                {
                    await problemWriter.WriteLineAsync(line);
                    _logger.LogWarning("Невалидная строка лога на строке {LineNumber}: {Line}", lineNumber, line);
                }
            }
            _logger.LogInformation("Обработка логов завершена.");
        }
    }
}
