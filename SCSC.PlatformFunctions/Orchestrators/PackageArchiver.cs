using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.PlatformFunctions.Orchestrators
{
    public class PackageArchiver
    {
        public class PackageArchiveInfo
        {
            public string ElfId { get; set; }
            public string ElfEntityName { get; set; }
            public IEnumerable<PackageInfoModel> Packages { get; set; }
        }

        public class ArchivePackage
        {
            public ArchivePackage()
            {

            }
            public ArchivePackage(string elfId, string elfEntityName, PackageInfoModel package)
            {
                ElfId = elfId;
                ElfEntityName = elfEntityName;
                EndTimestamp = package.EndTimestamp.HasValue ? package.EndTimestamp.Value.ToString("o") : string.Empty;
                GiftDescription = package.GiftDescription;
                KidName = package.KidName;
                PackageId = package.PackageId;
                StartTimestamp = package.StartTimestamp.ToString("O");
            }

            public string PartitionKey { get => this.ElfId; }
            public string RowKey { get => this.PackageId; }
            public string StartTimestamp { get; set; }
            public string EndTimestamp { get; set; }
            public string PackageId { get; set; }
            public string ElfId { get; set; }
            public string GiftDescription { get; set; }
            public string KidName { get; set; }
            public string ElfEntityName { get; set; }
        }


        [FunctionName(nameof(ArchivePackages))]
        public async Task ArchivePackages(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            logger.LogInformation($"[START ORCHESTRATOR] --> {nameof(PackageArchiver.ArchivePackage)}");
            var packagesInfo = context.GetInput<PackageArchiveInfo>();

            try
            {
                await context.CallActivityAsync(nameof(PackageArchiver.SavePackageToStorage), packagesInfo);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, $"Error during '{nameof(PackageArchiver.SavePackageToStorage)}' invocation", packagesInfo);
            }
        }

        [FunctionName(nameof(SavePackageToStorage))]
        public async Task SavePackageToStorage([ActivityTrigger] PackageArchiveInfo packagesInfo,
            [Table("packagesArchive", Connection = "PackagesStorageAccount")] IAsyncCollector<ArchivePackage> outputTable,
            ILogger logger)
        {
            logger.LogInformation($"[START ACTIVITY] --> {nameof(PackageArchiver.SavePackageToStorage)} for ${packagesInfo.ElfId} elf");

            if (packagesInfo.Packages != null && packagesInfo.Packages.Any())
            {
                foreach (var package in packagesInfo.Packages)
                {
                    var tablePackage = new ArchivePackage(packagesInfo.ElfId, packagesInfo.ElfEntityName, package);
                    await outputTable.AddAsync(tablePackage);
                }
            }
        }
    }
}
