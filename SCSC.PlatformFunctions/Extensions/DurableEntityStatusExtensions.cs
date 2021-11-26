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

        public static bool IsDeleted(this DurableEntityStatus entity)
        {
            if (entity == null)
                return true;

            var retVal = false;

            var jState = entity.State as JObject;
            if (jState != null)
            {
                var isDeletedProperty = jState.Property("isDeleted");
                if (isDeletedProperty!=null && isDeletedProperty.HasValues)
                {
                    bool.TryParse(isDeletedProperty.Value.ToString(), out retVal);
                }
            }

            return retVal;
        }
    }
}
