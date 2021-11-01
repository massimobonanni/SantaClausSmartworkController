using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SCSC.ElfSimulator
{
    internal class Program
    {
        private static Parameters _parameters;

        private static Random rand = new Random(DateTime.Now.Millisecond);

        static async Task Main(string[] args)
        {
            ParserResult<Parameters> result = Parser.Default.ParseArguments<Parameters>(args)
                .WithParsed(parsedParams =>
                {
                    _parameters = parsedParams;
                })
                .WithNotParsed(errors =>
                {
                    Environment.Exit(1);
                });

            if (_parameters.IsValid())
                Environment.Exit(1);

            WriteLog("Elf Simulator!");
            ElfsConfiguration config = await ReadConfigurationAsync(_parameters);

            WriteLog("Press control-C to exit.");

            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cts.Cancel();
                Console.WriteLine("Exiting...");
            };

            var tasks = new List<Task>();
            foreach (var elf in config.Elfs)
            {
                tasks.Add(SimulateElfActivitiesAsync(elf, cts.Token));
                await Task.Delay(rand.Next(125, 2000));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static async Task<ElfsConfiguration> ReadConfigurationAsync(Parameters parameters)
        {
            ElfsConfiguration config = null;
            if (_parameters.IsFileConfig())
            {
                config = await ReadFromFileConfigAsync(parameters);
            }
            else
            {
                config = await ReadFromBlobStorageAsync(parameters);
            }

            return config;
        }

        private static async Task<ElfsConfiguration> ReadFromBlobStorageAsync(Parameters parameters)
        {
            WriteLog($"Reading configuration from blob '{parameters.BlobUrl}'");

            var uri = new Uri(parameters.BlobUrl);

            using (var client = new HttpClient())
            {
                var configFile = await client.GetStringAsync(uri);
                return JsonSerializer.Deserialize<ElfsConfiguration>(configFile);
            }
        }

        private static async Task<ElfsConfiguration> ReadFromFileConfigAsync(Parameters parameters)
        {
            WriteLog($"Reading configuration from file '{parameters.ConfigFilePath}'");

            var configFile = await File.ReadAllTextAsync(parameters.ConfigFilePath);
            return JsonSerializer.Deserialize<ElfsConfiguration>(configFile);
        }

        private static object _logSyncObject = new object();

        private static void WriteLog(string message, ConsoleColor foregroundColor = ConsoleColor.White)
        {
            lock (_logSyncObject)
            {
                var actualForegroundColor = Console.ForegroundColor;
                Console.ForegroundColor = foregroundColor;
                Console.WriteLine(message);
                Console.ForegroundColor = actualForegroundColor;
            }
        }

        private static async Task SimulateElfActivitiesAsync(ElfConfiguration elf, CancellationToken ct)
        {
            
            var startupDelay = rand.Next(0, 10000);
            await Task.Delay(startupDelay, ct);

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    WriteLog($"{DateTime.Now} > Elf {elf.Name} Begin packaging",ConsoleColor.Green);
                    var package = "Begin"; // here add payload for start operation

                    var operationDuration = rand.Next(elf.OperationDurationMinInSec, elf.OperationDurationMaxInSec) * 1000;
                    await Task.Delay(operationDuration, ct);

                    WriteLog($"{DateTime.Now} > Elf {elf.Name} End packaging",ConsoleColor.Yellow);
                    package = "End"; // here add payload for start operation

                    await Task.Delay(elf.BreakDurationMaxInSec * 1000, ct);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

        }
    }
}
