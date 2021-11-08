using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Newtonsoft.Json.Linq
{
    public static class NewtonsoftExtensions
    {
        public static ElfInfoModel ToElfInfoModel(this JObject jobject)
        {
            if (jobject == null)
                return null;

            var retVal = new ElfInfoModel();
            retVal.Name = (string)jobject.Property("name").Value;
            retVal.LastUpdate = DateTimeOffset.Parse(jobject.Property("lastUpdate").Value.ToString());
            retVal.Packages = jobject.ToPackageInfoModels();

            return retVal;
        }
        public static List<PackageInfoModel> ToPackageInfoModels(this JObject jobject)
        {
            if (jobject == null)
                return null;

            List<PackageInfoModel> retVal = null;
            var packages = jobject.Property("packages").Value as JObject;
            if (packages != null)
            {
                var convertedPackages = packages.ToObject<List<PackageInfoModel>>();
                retVal = convertedPackages;
            }

            return retVal;
        }

    }
}
