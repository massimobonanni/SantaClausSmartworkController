using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCSC.Core.Test.Utilities
{
    internal static class PackageInfoModelUtility
    {
        public static PackageInfoModel GenerateClosedPackage(DateTimeOffset startTime, int durationInSecs = 10)
        {
            PackageInfoModel model = GenerateOpenedPackage(startTime);
            model.EndTimestamp = startTime.AddSeconds(durationInSecs);
            return model;
        }

        public static PackageInfoModel GenerateOpenedPackage(DateTimeOffset startTime)
        {
            PackageInfoModel model = new PackageInfoModel();
            model.StartTimestamp = startTime;
            return model;
        }
    }
}
