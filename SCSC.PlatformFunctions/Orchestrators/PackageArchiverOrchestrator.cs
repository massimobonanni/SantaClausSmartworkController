using Microsoft.Azure.Cosmos.Table;
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
    internal class PackageArchiverOrchestrator
    {
        public class PackageArchiveInfo
        {
            public string ElfId { get; set; }
            public string ElfEntityName { get; set; }
            public string ElfName { get; set; }
            public List<PackageInfoModel> Packages { get; set; }
        }

        public class ArchivePackage : TableEntity
        {
            public ArchivePackage()
            {

            }
            public ArchivePackage(string elfId, string elfEntityName, string elfName, PackageInfoModel package)
            {
                PartitionKey = elfId;
                RowKey = package.PackageId;
                ElfEntityName = elfEntityName;
                EndTimestamp = package.EndTimestamp.HasValue ? package.EndTimestamp.Value.ToString("o") : string.Empty;
                GiftDescription = package.GiftDescription;
                KidName = package.KidName;
                StartTimestamp = package.StartTimestamp.ToString("O");
                ElfName = elfName;
            }

            public string StartTimestamp { get; set; }
            public string EndTimestamp { get; set; }
            public string PackageId { get => RowKey; }
            public string ElfId { get => PartitionKey; }
            public string GiftDescription { get; set; }
            public string KidName { get; set; }
            public string ElfName { get; set; }
            public string ElfEntityName { get; set; }
        }


        [FunctionName(nameof(ArchivePackages))]
        public async Task ArchivePackages(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            logger.LogInformation($"[START ORCHESTRATOR] --> {nameof(PackageArchiverOrchestrator.ArchivePackage)}");
            var packagesInfo = context.GetInput<PackageArchiveInfo>();
            
            try
            {
                await context.CallActivityAsync(nameof(PackageArchiverOrchestrator.SavePackageToStorage), packagesInfo);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex, $"Error during '{nameof(PackageArchiverOrchestrator.SavePackageToStorage)}' invocation", packagesInfo);
            }
        }

        [FunctionName(nameof(SavePackageToStorage))]
        public async Task SavePackageToStorage([ActivityTrigger] PackageArchiveInfo packagesInfo,
            [Table("packagesArchive", Connection = "PackagesStorageAccount")] IAsyncCollector<ArchivePackage> outputTable,
            ILogger logger)
        {
            logger.LogInformation($"[START ACTIVITY] --> {nameof(PackageArchiverOrchestrator.SavePackageToStorage)} for {packagesInfo.ElfId} elf");

            if (packagesInfo.Packages != null && packagesInfo.Packages.Any())
            {
                foreach (var package in packagesInfo.Packages)
                {
                    var tablePackage = new ArchivePackage(packagesInfo.ElfId, packagesInfo.ElfEntityName, packagesInfo.ElfName, package);
                    await outputTable.AddAsync(tablePackage);
                }
            }
        }
    }
}
