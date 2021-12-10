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
        #region [ Method CalculateHourlyProductivity]
        [Fact]
        public void CalculateHourlyProductivity_ArgumentNull_ReturnZero()
        {
            IEnumerable<PackageInfoModel> target = null;

            var actualProductivity = target.CalculateHourlyProductivity(new TimeSpan(9, 0, 0), new TimeSpan(18, 0, 0));

            Assert.Equal(0.0, actualProductivity);
        }


        [Theory()]
        [MemberData(nameof(PackageInfoModelDataGenerator.GetPackagesForProductivity),
            MemberType = typeof(PackageInfoModelDataGenerator))]
        public void CalculateHourlyProductivity_CalcutateAverageSpeed(
            IEnumerable<PackageInfoModel> packages, DateTimeOffset calculationTime,
            TimeSpan startWorkTime, TimeSpan endWorkTime, double expectedHourlyProd,double expectedDailyProd)
        {
            var actualProductivity = packages.CalculateHourlyProductivity(startWorkTime, endWorkTime, calculationTime);
            Assert.Equal(expectedHourlyProd, actualProductivity);
        }
        #endregion

        #region [ Method CalculateDailyProductivity]
        [Fact]
        public void CalculateDailyProductivity_ArgumentNull_ReturnZero()
        {
            IEnumerable<PackageInfoModel> target = null;

            var actualProductivity = target.CalculateDailyProductivity(new TimeSpan(9, 0, 0), new TimeSpan(18, 0, 0));

            Assert.Equal(0.0, actualProductivity);
        }


        [Theory()]
        [MemberData(nameof(PackageInfoModelDataGenerator.GetPackagesForProductivity),
            MemberType = typeof(PackageInfoModelDataGenerator))]
        public void CalculateDailyProductivity_CalcutateAverageSpeed(
            IEnumerable<PackageInfoModel> packages, DateTimeOffset calculationTime,
            TimeSpan startWorkTime, TimeSpan endWorkTime, double expectedHourlyProd, double expectedDailyProd)
        {
            var actualProductivity= packages.CalculateDailyProductivity(startWorkTime, endWorkTime, calculationTime);
            Assert.Equal(expectedDailyProd, actualProductivity);
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
