using Newtonsoft.Json.Linq;
using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.WebJobs.Extensions.DurableTask
{
    public static class DurableEntityStatusExtensions
    {
        public static ElfInfoModel ToElfInfoModel(this DurableEntityStatus entity)
        {
            if (entity == null)
                return null;

            var retVal = new ElfInfoModel();

            var jobject = entity.State as JObject;
            if (jobject != null)
            {
                retVal = jobject.ToElfInfoModel();
            }
            retVal.Id = entity.EntityId.EntityKey;

            return retVal;
        }
    }
}
