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
        public void CalculateAverageSpeed_ArgumentNull_ThrowException()
        {
            IEnumerable<PackageInfoModel> target = null;

            Assert.Throws<NullReferenceException>(() => target.CalcuateAverageSpeed());
        }

        [Theory()]
        [MemberData(nameof(PackageInfoModelDataGenerator.GetPackagesForAverageSpeedWithTimeReference),
            MemberType=typeof(PackageInfoModelDataGenerator))]
        public void CalculateAverageSpeed_CalcutateAverageSpeedWithTimeReference(
            IEnumerable<PackageInfoModel> packages, TimeSpan timeReference, double expectedSpeed)
        {
            var actualSpeed = packages.CalcuateAverageSpeed(timeReference);
            Assert.Equal(expectedSpeed, actualSpeed);
        }

        [Theory()]
        [MemberData(nameof(PackageInfoModelDataGenerator.GetPackagesForAverageSpeed),
            MemberType = typeof(PackageInfoModelDataGenerator))]
        public void CalculateAverageSpeed_CalcutateAverageSpeed(
            IEnumerable<PackageInfoModel> packages, double expectedSpeed)
        {
            var actualSpeed = packages.CalcuateAverageSpeed();
            Assert.Equal(expectedSpeed, actualSpeed);
        }
        #endregion

    }
}
