using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
    {
        public static int CountPackagesInDay(this IEnumerable<PackageInfoModel> packages, DateTime date)
        {
            if (packages == null)
                return 0;

            return packages.GetPackagesInDay(date).Count();
        }

        public static IEnumerable<PackageInfoModel> GetPackagesInDay(this IEnumerable<PackageInfoModel> packages,
            DateTime date)
        {
            if (packages == null)
                return null;

            var packagesInDay = packages
               .Where(p => p.StartTimestamp.Date == date.Date);

            return packagesInDay;
        }

        public static double CalculateAverageSpeed(this IEnumerable<PackageInfoModel> packages, TimeSpan startworkTime,
            TimeSpan endWorkTime, DateTimeOffset? calculationTime = null, TimeSpan? timeReference = null)
        {
            if (packages == null)
                return 0;

            var tNow = DateTimeOffset.UtcNow; // Time to calculate the average

            if (calculationTime.HasValue)
                tNow = calculationTime.Value;
            if (!timeReference.HasValue)
                timeReference = TimeSpan.FromHours(1);

            var tStartCalc = tNow.Subtract(timeReference.Value); // Initial time to calculate the average
            var tEnd = tNow.TimeOfDay < endWorkTime ? tNow.TimeOfDay : endWorkTime;
            var tStart = tStartCalc.TimeOfDay < startworkTime ? startworkTime : tStartCalc.TimeOfDay;

            var packagesInTime = packages
                .Where(p => p.StartTimestamp > tStartCalc)
                .Where(p => p.EndTimestamp.HasValue && p.EndTimestamp.Value <= tNow);

            var speed = packagesInTime.Count() / Math.Abs(tEnd.Subtract(tStart).TotalHours);

            return speed;
        }

        public static IEnumerable<PackageInfoModel> ExtractOldItems(this IEnumerable<PackageInfoModel> packages,
            DateTime? fromDate = null, int? maxSize = null)
        {
            if (packages == null)
                throw new NullReferenceException(nameof(packages));

            if (!fromDate.HasValue)
            {
                fromDate = DateTimeOffset.UtcNow.AddDays(-1).Date;
            }

            var oldPackages = packages
                .Where(p => p.StartTimestamp.Date <= fromDate.Value)
                .Where(p => !p.IsOpen);

            if (maxSize.HasValue)
                oldPackages = oldPackages
                    .OrderBy(p => p.StartTimestamp)
                    .Take(maxSize.Value);

            return oldPackages;
        }
    }
}
