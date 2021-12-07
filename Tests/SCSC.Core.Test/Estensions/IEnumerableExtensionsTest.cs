using SCSC.Core.Models;
using SCSC.Core.Test.DataGenerators;
using SCSC.Core.Test.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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

        #region [ Method GetPackagesInDay ]
        [Fact]
        public void GetPackagesInDay_ArgumentNull_null()
        {
            IEnumerable<PackageInfoModel> target = null;

            var actual = target.GetPackagesInDay(DateTime.Now);

            Assert.Null(actual);
        }

        [Theory()]
        [MemberData(nameof(PackageInfoModelDataGenerator.GetPackagesForPackagesInDay),
            MemberType = typeof(PackageInfoModelDataGenerator))]
        public void GetPackagesInDay_Calcutate(
            IEnumerable<PackageInfoModel> packages, DateTime date, int expectedCount)
        {
            var actualPackages = packages.GetPackagesInDay(date);
            Assert.Equal(expectedCount, actualPackages.Count());
        }
        #endregion [ Method GetPackagesInDay ]

        #region [ Method CountPackagesInDay ]
        [Fact]
        public void CountPackagesInDay_ArgumentNull_null()
        {
            IEnumerable<PackageInfoModel> target = null;

            var actual = target.CountPackagesInDay(DateTime.Now);

            Assert.Equal(0, actual);
        }

        [Theory()]
        [MemberData(nameof(PackageInfoModelDataGenerator.GetPackagesForPackagesInDay),
            MemberType = typeof(PackageInfoModelDataGenerator))]
        public void CountPackagesInDay_Calcutate(
            IEnumerable<PackageInfoModel> packages, DateTime date, int expectedCount)
        {
            var actualCount = packages.CountPackagesInDay(date);
            Assert.Equal(expectedCount, actualCount);
        }
        #endregion [ Method CountPackagesInDay ]
    }
}
