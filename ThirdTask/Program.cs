using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using ThirdTask.Service;

namespace ThirdTask
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("Консольная программа для стандартизации лог-файлов");

            Option<FileInfo> inputOption = new("--input", "-i")
            {
                Description = "Путь к входному лог-файлу",
                DefaultValueFactory = _ => new FileInfo("input.log")
            };

            Option<FileInfo> outputOption = new("--output", "-o")
            {
                Description = "Путь к выходному файлу",
                DefaultValueFactory = _ => new FileInfo("output.log")
            };

            Option<FileInfo> problemOption = new("--problems", "-p")
            {
                Description = "Путь к файлу с невалидными строками",
                DefaultValueFactory = _ => new FileInfo("problems.txt")
            };

            rootCommand.Options.Add(inputOption);
            rootCommand.Options.Add(outputOption);
            rootCommand.Options.Add(problemOption);

            rootCommand.SetAction(async (ParseResult parseResult, CancellationToken cancellationToken) =>
            {
                var input = parseResult.GetValue(inputOption);
                var output = parseResult.GetValue(outputOption);
                var problems = parseResult.GetValue(problemOption);

                // Создаём Host внутри действия
                var hostBuilder = Host.CreateDefaultBuilder(Array.Empty<string>()) // args уже обработаны CLI
                    .ConfigureLogging(logging =>
                    {
                        logging.AddFilter("Microsoft.Hosting.Lifetime", Microsoft.Extensions.Logging.LogLevel.Warning);
                    })
                    .ConfigureServices((context, services) =>
                    {
                        // Регистрация ваших сервисов
                        services.AddTransient<ILogProcessor, LogProcessor>();
                        services.AddTransient<ILogParserFactory, LogParserFactory>();
                    });

                using var host = hostBuilder.Build();

                await host.StartAsync(cancellationToken);

                var logProcessor = host.Services.GetRequiredService<ILogProcessor>();

                await logProcessor.ProcessAsync(input.FullName, output.FullName, problems.FullName);

                await host.StopAsync(cancellationToken);

                return 0;
            });

            ParseResult parseResult = rootCommand.Parse(args);

            return await parseResult.InvokeAsync(); 
        }
    }
}
