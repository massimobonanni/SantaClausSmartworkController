﻿using SCSC.Core.Models;
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
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-20))
                    },
                DateTimeUtility.Create(14,0,0),
                TimeSpan.FromHours(1),
                new TimeSpan(9,0,0),
                new TimeSpan(18,0,0),
                1.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-20)),
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-10))
                    },
                DateTimeUtility.Create(14,0,0),
                TimeSpan.FromHours(1),
                new TimeSpan(9,0,0),
                new TimeSpan(18,0,0),
                2.0
            };

            yield return new object[]
           {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-20)),
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-10)),
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-30))
                    },
                DateTimeUtility.Create(14,0,0),
                TimeSpan.FromHours(1),
                new TimeSpan(9,0,0),
                new TimeSpan(18,0,0),
                3.0
           };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateOpenedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-20))
                    },
                DateTimeUtility.Create(14,0,0),
                TimeSpan.FromHours(1),
                new TimeSpan(9,0,0),
                new TimeSpan(18,0,0),
                0.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateOpenedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-20))
                    },
                DateTimeUtility.Create(14,0,0),
                TimeSpan.FromMinutes(30),
                new TimeSpan(9,0,0),
                new TimeSpan(18,0,0),
                0.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-20))
                    },
                DateTimeUtility.Create(14,0,0),
                TimeSpan.FromMinutes(30),
                new TimeSpan(9,0,0),
                new TimeSpan(18,0,0),
                2.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-40))
                    },
                DateTimeUtility.Create(14,0,0),
                TimeSpan.FromMinutes(30),
                new TimeSpan(9,0,0),
                new TimeSpan(18,0,0),
                0.0
            };
        }

        public static IEnumerable<object[]> GetPackagesForAverageSpeed()
        {
            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-30))
                    },
                DateTimeUtility.Create(14,0,0),
                new TimeSpan(9,0,0),
                new TimeSpan(18,0,0),
                1.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-20)),
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-10)),
                    },
                DateTimeUtility.Create(14,0,0),
                new TimeSpan(9,0,0),
                new TimeSpan(18,0,0),
                2.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-20)),
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-1),600),
                    },
                DateTimeUtility.Create(14,0,0),
                new TimeSpan(9,0,0),
                new TimeSpan(18,0,0),
                1.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-20)),
                        PackageInfoModelUtility.GenerateOpenedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-10)),
                    },
                DateTimeUtility.Create(14,0,0),
                new TimeSpan(9,0,0),
                new TimeSpan(18,0,0),
                1.0
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateOpenedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-20)),
                        PackageInfoModelUtility.GenerateOpenedPackage(DateTimeUtility.Create(14,0,0).AddMinutes(-10)),
                    },
                DateTimeUtility.Create(14,0,0),
                new TimeSpan(9,0,0),
                new TimeSpan(18,0,0),
                0.0
            };
        }

        public static IEnumerable<object[]> GetPackagesForPackagesInDay()
        {
            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now)
                    },
                DateTimeOffset.Now.DateTime,
                1
            };
            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddHours(-1)),
                        PackageInfoModelUtility.GenerateOpenedPackage(DateTimeOffset.Now)
                    },
                DateTimeOffset.Now.DateTime,
                2
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddDays(-1)),
                        PackageInfoModelUtility.GenerateOpenedPackage(DateTimeOffset.Now)
                    },
                DateTimeOffset.Now.DateTime,
                1
            };

            yield return new object[]
            {
                new List<PackageInfoModel>()
                    {
                        PackageInfoModelUtility.GenerateClosedPackage(DateTimeOffset.Now.AddDays(-1)),
                        PackageInfoModelUtility.GenerateOpenedPackage(DateTimeOffset.Now.AddDays(-2))
                    },
                DateTimeOffset.Now.DateTime,
                0
            };
        }
    }
}
