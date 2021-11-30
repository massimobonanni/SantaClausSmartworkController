using SCSC.Core.Models;
using SCSC.Core.Test.DataGenerators;
using SCSC.Core.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SCSC.Core.Test.Estensions
{
    public class IEnumerableExtensionsTest
    {
        #region [ Method CalculateAverageSpeed]
        [Fact]
        public void CalculateAverageSpeed_ArgumentNull_ReturnZero()
        {
            IEnumerable<PackageInfoModel> target = null;

            var actual = target.CalculateAverageSpeed(new TimeSpan(9, 0, 0), new TimeSpan(18, 0, 0));

            Assert.Equal(0.0, actual);
        }

        [Theory()]
        [MemberData(nameof(PackageInfoModelDataGenerator.GetPackagesForAverageSpeedWithTimeReference),
            MemberType = typeof(PackageInfoModelDataGenerator))]
        public void CalculateAverageSpeed_CalcutateAverageSpeedWithTimeReference(
            IEnumerable<PackageInfoModel> packages, DateTimeOffset calculationTime, TimeSpan timeReference,
            TimeSpan startWorkTime, TimeSpan endWorkTime, double expectedSpeed)
        {
            var actualSpeed = packages.CalculateAverageSpeed(startWorkTime, endWorkTime, calculationTime, timeReference);
            Assert.Equal(expectedSpeed, actualSpeed);
        }

        [Theory()]
        [MemberData(nameof(PackageInfoModelDataGenerator.GetPackagesForAverageSpeed),
            MemberType = typeof(PackageInfoModelDataGenerator))]
        public void CalculateAverageSpeed_CalcutateAverageSpeed(
            IEnumerable<PackageInfoModel> packages, DateTimeOffset calculationTime,
            TimeSpan startWorkTime, TimeSpan endWorkTime, double expectedSpeed)
        {
            var actualSpeed = packages.CalculateAverageSpeed(startWorkTime, endWorkTime, calculationTime);
            Assert.Equal(expectedSpeed, actualSpeed);
        }
        #endregion

    }
}
