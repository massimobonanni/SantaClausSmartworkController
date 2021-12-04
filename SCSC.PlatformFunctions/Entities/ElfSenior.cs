using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SCSC.Core.Models;
using SCSC.PlatformFunctions.Entities.Interfaces;
using SCSC.PlatformFunctions.Orchestrators;
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
        [JsonIgnore]
        private readonly ILogger logger;

        public ElfSenior(ILogger logger)
        {
            this.logger = logger;
        }

        #region [ State ]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lastUpdate")]
        public DateTimeOffset? LastUpdate { get; set; }

        [JsonProperty("lastPackages")]
        public List<PackageInfoModel> Packages { get; set; } = new List<PackageInfoModel>();

        [JsonProperty("startWorkTime")]
        public TimeSpan StartWorkTime { get; set; } = new TimeSpan(9, 0, 0);

        [JsonProperty("endWorkTime")]
        public TimeSpan EndWorkTime { get; set; } = new TimeSpan(18, 0, 0);

        [JsonProperty("isDeleted")]
        public bool IsDeleted { get; set; } = false;
        #endregion [ State ]

        #region [ IElfEntity interface ]
        public async void Configure(ElfConfigurationModel config)
        {
            if (config != null)
            {
                this.Name = config.Name;
                this.StartWorkTime = TimeSpan.Parse(config.StartWorkTime);
                this.EndWorkTime = TimeSpan.Parse(config.EndWorkTime);
                this.IsDeleted = false;
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
                    this.LastUpdate = DateTimeOffset.Now;
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
                    this.LastUpdate = DateTimeOffset.Now.ToUniversalTime();
                }
            }

            await CleanPackagesAsync();
        }

        public Task<double> GetHourProductivity()
        {
            if (this.Packages == null)
                return Task.FromResult(0.0);

            var productivity = this.Packages.CalculateAverageSpeed(this.StartWorkTime,
                this.EndWorkTime, null, TimeSpan.FromHours(1));

            return Task.FromResult(productivity);
        }

        public Task<DateTimeOffset?> GetLastUpdate()
        {
            return Task.FromResult(this.LastUpdate);
        }

        public void Delete()
        {
            this.Name = null;
            this.StartWorkTime = TimeSpan.Zero;
            this.EndWorkTime = TimeSpan.Zero;
            this.Packages = new List<PackageInfoModel>();
            this.IsDeleted = true;
        }

        #endregion [ IElfEntity interface ]

        #region [ Internal Methods ]
        private Task CleanPackagesAsync()
        {
            var packagesToRemove = this.Packages.ExtractOldItems(TimeSpan.FromDays(1)).ToList();

            if (packagesToRemove.Any())
            {
                this.logger.LogInformation($"Calling orchestrator {nameof(PackageArchiverOrchestrator.ArchivePackages)}");
                var packageArchive = new PackageArchiverOrchestrator.PackageArchiveInfo()
                {
                    ElfId = Entity.Current.EntityKey,
                    ElfEntityName = Entity.Current.EntityName,
                    ElfName = this.Name,
                    Packages = packagesToRemove
                };
                Entity.Current.StartNewOrchestration(nameof(PackageArchiverOrchestrator.ArchivePackages), packageArchive);
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
