using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
    {

        public static double CalcuateAverageSpeed(this IEnumerable<PackageInfoModel> packages, TimeSpan startworkTime,
            TimeSpan endWorkTime, DateTimeOffset? calculationTime = null, TimeSpan? timeReference = null)
        {
            if (packages == null)
                throw new NullReferenceException(nameof(packages));

            var tNow = DateTimeOffset.Now; // Time to calculate the average

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
            TimeSpan oldItemTimeThreshold)
        {
            if (packages == null)
                throw new NullReferenceException(nameof(packages));

            var oldPackages = packages
                .Where(p => p.StartTimestamp < DateTimeOffset.Now.Subtract(oldItemTimeThreshold))
                .Where(p => !p.IsOpen);

            return oldPackages;
        }
    }
}
