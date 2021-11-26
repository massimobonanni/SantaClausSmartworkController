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
            if (jobject.Property("name") != null)
                retVal.Name = (string)jobject.Property("name").Value;
            if (jobject.Property("startWorkTime") != null &&
                TimeSpan.TryParse(jobject.Property("startWorkTime").Value.ToString(),out var tmpStartWorkTime))
                retVal.StartWorkTime = tmpStartWorkTime;
            if (jobject.Property("endWorkTime") != null &&
                TimeSpan.TryParse(jobject.Property("endWorkTime").Value.ToString(), out var tmpEndWorkTime))
                retVal.EndWorkTime = tmpEndWorkTime;
            if (jobject.Property("lastUpdate") != null &&
                DateTimeOffset.TryParse(jobject.Property("lastUpdate").Value.ToString(), out var tmpLastUpdate))
                retVal.LastUpdate = tmpLastUpdate;
            retVal.Packages = jobject.ToPackageInfoModels();

            return retVal;
        }
        public static List<PackageInfoModel> ToPackageInfoModels(this JObject jobject)
        {
            if (jobject == null)
                return null;

            List<PackageInfoModel> retVal = null;
            if (jobject.Property("lastPackages") != null)
            {
                var packages = jobject.Property("lastPackages").Value as JArray;
                if (packages != null)
                {
                    var convertedPackages = packages.ToObject<List<PackageInfoModel>>();
                    retVal = convertedPackages;
                }
            }
            return retVal;
        }

    }
}
