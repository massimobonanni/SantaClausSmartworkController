using SCSC.Core.Models;
using SCSC.Core.Test.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SCSC.Core.Test.DataGenerators
{
    public class PackageInfoModelDataGenerator
    {
        public static IEnumerable<object[]> GetPackagesForAverageSpeedWithTimeReference()
        {
            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-20))
                    },
                TimeSpan.FromHours(1),
                1.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-20)),
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-10))
                    },
                TimeSpan.FromHours(1),
                2.0
            };

            yield return new object[]
           {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-20)),
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-10)),
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-30))
                    },
                TimeSpan.FromHours(1),
                3.0
           };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateOpenedPackage(DateTimeOffset.Now.AddMinutes(-20))
                    },
                TimeSpan.FromHours(1),
                0.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateOpenedPackage(DateTimeOffset.Now.AddMinutes(-20))
                    },
                TimeSpan.FromMinutes(30),
                0.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-20))
                    },
                TimeSpan.FromMinutes(30),
                2.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-40))
                    },
                TimeSpan.FromMinutes(30),
                0.0
            };
        }

        public static IEnumerable<object[]> GetPackagesForAverageSpeed()
        {
            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-30))
                    },
                1.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-20)),
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-10)),
                    },
                2.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-20)),
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-1),600),
                    },
                1.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddMinutes(-20)),
                        PackageInfoModelUtility.GenerateOpenedPackage(DateTimeOffset.Now.AddMinutes(-10)),
                    },
                1.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateOpenedPackage(DateTimeOffset.Now.AddMinutes(-20)),
                        PackageInfoModelUtility.GenerateOpenedPackage(DateTimeOffset.Now.AddMinutes(-10)),
                    },
                0.0
            };
        }
    }
}
