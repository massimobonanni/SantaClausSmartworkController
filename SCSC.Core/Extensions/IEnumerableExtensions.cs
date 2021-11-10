using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
    {

        public static double CalcuateAverageSpeed(this IEnumerable<PackageInfoModel> packages, TimeSpan? timeReference = null)
        {
            if (packages == null)
                throw new NullReferenceException(nameof(packages));

            if (!timeReference.HasValue)
                timeReference = TimeSpan.FromHours(1);

            var packagesInTime = packages
                .Where(p => p.StartTimestamp > DateTimeOffset.Now.Subtract(timeReference.Value))
                .Where(p=>p.EndTimestamp.HasValue && p.EndTimestamp.Value<=DateTimeOffset.Now);

            var speed = packagesInTime.Count() / timeReference.Value.TotalHours;

            return speed;
        }

        public static IEnumerable<PackageInfoModel> ExtractOldItems(this IEnumerable<PackageInfoModel> packages, 
            TimeSpan oldItemTimeThreshold)
        {
            if (packages == null)
                throw new NullReferenceException(nameof(packages));

            var oldPackages = packages
                .Where(p => p.StartTimestamp< DateTimeOffset.Now.Subtract(oldItemTimeThreshold))
                .Where(p => !p.IsOpen);
                        
            return oldPackages;
        }
    }
}
