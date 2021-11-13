using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using static SCSC.PlatformFunctions.Orchestrators.PackageArchiverOrchestrator;

namespace SCSC.PlatformFunctions.Orchestrators
{
    internal static class ArchivePackageExtensions
    {
        public static PackageDetailModel ToDetailModel(this ArchivePackage source)
        {
            return new PackageDetailModel()
            {
                ElfId = source.ElfId,
                ElfName = source.ElfName,
                EndTimestamp = DateTimeOffset.Parse(source.EndTimestamp),
                GiftDescription = source.GiftDescription,
                KidName = source.KidName,
                PackageId = source.PackageId,
                StartTimestamp = DateTimeOffset.Parse(source.EndTimestamp)
            };
        }
    }
}
