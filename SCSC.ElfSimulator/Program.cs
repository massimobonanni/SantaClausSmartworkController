using CommandLine;
using SCSC.APIClient;
using SCSC.Core.Models;
using SCSC.ElfSimulator.Fakers;
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

        private static ElfsRestClient restClient;

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

            using var httpClient = new HttpClient();
            restClient = new ElfsRestClient(httpClient, config.ApiBaseUrl, config.ApiKey);

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

        private enum ElfStatus
        {
            Starting,
            Break,
            Packaging,
            OutOfOffice
        }

        private static async Task SimulateElfActivitiesAsync(ElfConfiguration elf, CancellationToken ct)
        {
            var elfStatus = ElfStatus.Starting;
            var startJobTime = TimeSpan.Parse(elf.StartWorkTime);
            var endJobTime = TimeSpan.Parse(elf.EndWorkTime);

            await restClient.UpdateElfAsync(elf.Id, new UpdateElfModel()
            {
                Configuration = new ElfConfigurationModel()
                {
                    Name = elf.Name,
                    StartWorkTime = elf.StartWorkTime,
                    EndWorkTime = elf.EndWorkTime
                }
            }, default);

            WriteLog($"{DateTime.Now} > Elf {elf.Name} is starting its job", ConsoleColor.Cyan);
            var startupDelay = rand.Next(0, 10000);
            await Task.Delay(startupDelay, ct);

            while (!ct.IsCancellationRequested)
            {
                PackageStartedModel packageStarted = null;
                PackageEndedModel packageEnded = null;
                try
                {
                    if (DateTimeOffset.Now.ToUniversalTime().TimeOfDay >= startJobTime && DateTimeOffset.Now.ToUniversalTime().TimeOfDay <= endJobTime)
                    {
                        WriteLog($"{DateTime.Now} > Elf {elf.Name} begin packaging", ConsoleColor.Green);
                        packageStarted = PackageFaker.PackageStarted();
                        await restClient.PackageStartedAsync(elf.Id, packageStarted, default);

                        elfStatus = ElfStatus.Packaging;
                        var operationDuration = rand.Next(elf.OperationDurationMinInSec, elf.OperationDurationMaxInSec) * 1000;
                        await Task.Delay(operationDuration, ct);

                        WriteLog($"{DateTime.Now} > Elf {elf.Name} end packaging", ConsoleColor.Yellow);
                        packageEnded = new PackageEndedModel()
                        {
                            PackageId = packageStarted.PackageId,
                            Timestamp = DateTimeOffset.Now.ToUniversalTime()
                        };
                        await restClient.PackageEndedAsync(elf.Id, packageEnded, default);

                        elfStatus = ElfStatus.Break;
                        WriteLog($"{DateTime.Now} > Elf {elf.Name} is starting its break", ConsoleColor.Magenta);
                    }
                    else
                    {
                        if (elfStatus != ElfStatus.OutOfOffice)
                        {
                            elfStatus = ElfStatus.OutOfOffice;
                            WriteLog($"{DateTime.Now} > Elf {elf.Name} is out of office", ConsoleColor.Red);
                        }
                    }

                    var breakDuration = CalculateBreakDuration(elf);

                    await Task.Delay(breakDuration, ct);
                }
                catch (TaskCanceledException)
                {
                    if (elfStatus == ElfStatus.Packaging)
                    {
                        packageEnded = new PackageEndedModel()
                        {
                            PackageId = packageStarted.PackageId,
                            Timestamp = DateTimeOffset.Now.ToUniversalTime()
                        };
                        await restClient.PackageEndedAsync(elf.Id, packageEnded, default);
                    }
                    break;
                }
            }

        }

        private static int CalculateBreakDuration(ElfConfiguration elf)
        {
            int duration = 0;
            var lazyProbability = rand.Next(0, 100);
            duration = rand.Next(1, elf.BreakDurationMaxInSec) * 1000;
            if (elf.LazyPercentage != 0 && elf.LazyPercentage >= lazyProbability)
            {
                WriteLog($"{DateTime.Now} > Elf {elf.Name} is lazy, he does not want to work", ConsoleColor.Blue);
                duration *= 20;
            }
            return duration;
        }
    }
}
