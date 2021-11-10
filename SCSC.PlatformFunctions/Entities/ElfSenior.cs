using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SCSC.Core.Models;
using SCSC.PlatformFunctions.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.PlatformFunctions.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ElfSenior : IElfEntity
    {
        private readonly ILogger logger;

        public ElfSenior(ILogger logger)
        {
            this.logger = logger;
        }

        #region [ State ]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lastUpdate")]
        public DateTimeOffset LastUpdate { get; set; }

        [JsonProperty("LastPackages")]
        public List<PackageInfoModel> Packages { get; set; } = new List<PackageInfoModel>();
        #endregion [ State ]

        #region [ IElfEntity interface ]
        public async void Configure(ElfConfigurationModel config)
        {
            if (config != null)
            {
                this.Name = config.Name;
            }

            await CleanPackagesAsync();
        }

        public async void PackageEnded(PackageEndedModel package)
        {
            if (package != null)
            {
                var innerPackage = this.Packages.FirstOrDefault(p => p.PackageId == package.PackageId);

                if (innerPackage != null && innerPackage.IsOpen)
                {
                    innerPackage.EndTimestamp = package.Timestamp;
                }
            }

            await CleanPackagesAsync();
        }

        public async void PackageStarted(PackageStartedModel package)
        {
            if (package != null)
            {
                var innerPackage = this.Packages.FirstOrDefault(p => p.PackageId == package.PackageId);

                if (innerPackage == null)
                {
                    innerPackage = new PackageInfoModel();
                    innerPackage.StartTimestamp = package.Timestamp;
                    innerPackage.GiftDescription = package.GiftDescription;
                    innerPackage.KidName = package.KidName;
                    innerPackage.PackageId = package.PackageId;
                    this.Packages.Add(innerPackage);
                }
            }

            await CleanPackagesAsync();
        }
        #endregion [ IElfEntity interface ]

        #region [ Internal Methods ]
        private Task CleanPackagesAsync()
        {
            var packagesToRemove = this.Packages.ExtractOldItems(TimeSpan.FromDays(1));

            if (packagesToRemove.Any())
            {
                this.logger.LogInformation("Removing old packages");
                // here put the orchestrator call to save package in db

                this.Packages.RemoveAll(p => packagesToRemove.Contains(p));
            }

            return Task.CompletedTask;
        }
        #endregion [ Internal Methods ]

        [FunctionName(nameof(ElfSenior))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx, ILogger logger)
            => ctx.DispatchAsync<ElfSenior>(logger);
    }
}
