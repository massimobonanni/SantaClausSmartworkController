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

        public static double CalculateHourlyProductivity(this IEnumerable<PackageInfoModel> packages,
            TimeSpan startworkTime,
            TimeSpan endWorkTime,
            DateTimeOffset? calculationTime = null)
        {
            if (packages == null)
                return 0;

            var tNow = calculationTime.HasValue ? calculationTime.Value : DateTimeOffset.UtcNow;
            var tStartCalc = tNow.AddHours(-1);
            var tEnd = tNow.TimeOfDay < endWorkTime ? tNow.TimeOfDay : endWorkTime;
            var tStart = tStartCalc.TimeOfDay;
            if (tStartCalc.TimeOfDay < startworkTime)
            {
                tStartCalc = new DateTimeOffset(tNow.Date.Year, tNow.Date.Month, tNow.Date.Day,
                    startworkTime.Hours, startworkTime.Minutes, startworkTime.Seconds, TimeSpan.Zero).ToUniversalTime();
                tStart = startworkTime;
            }

            var packagesInTime = packages
                .Where(p => !(p.StartTimestamp >=tNow ||
                               p.EndTimestamp.HasValue && p.EndTimestamp.Value<=tStartCalc))
                .Count();

            var productivity = packagesInTime * 3600 / tEnd.Subtract(tStart).TotalSeconds;
            return Math.Round(productivity, 2);
        }

        public static double CalculateDailyProductivity(this IEnumerable<PackageInfoModel> packages,
            TimeSpan startworkTime,
            TimeSpan endWorkTime,
            DateTimeOffset? calculationTime = null)
        {
            if (packages == null)
                return 0;

            var tNow = calculationTime.HasValue ? calculationTime.Value : DateTimeOffset.UtcNow;
            var tEnd = tNow.TimeOfDay < endWorkTime ? tNow.TimeOfDay : endWorkTime;
            var tStart = startworkTime;
            var tStartCalc = new DateTimeOffset(tNow.Date.Year, tNow.Date.Month, tNow.Date.Day,
                startworkTime.Hours, startworkTime.Minutes, startworkTime.Seconds, TimeSpan.Zero).ToUniversalTime();
            var workDayDuration = endWorkTime.Subtract(startworkTime).TotalSeconds;

            var packagesInTime = packages
                .Where(p => !(p.StartTimestamp >= tNow || 
                                p.EndTimestamp.HasValue && p.EndTimestamp.Value <= tStartCalc))
                .Count();

            var productivity = packagesInTime * workDayDuration / tEnd.Subtract(tStart).TotalSeconds;
            return Math.Round(productivity, 2);
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
