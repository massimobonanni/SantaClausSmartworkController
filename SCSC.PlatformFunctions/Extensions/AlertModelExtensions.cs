using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCSC.Core.Models
{
    internal static class AlertModelExtensions
    {

        public static T ExtractFromData<T>(this CreateAlertModel source)
        {
            try
            {
                if (source.Data is JObject)
                {
                    return ((JObject)source.Data).ToObject<T>();
                }
                else
                {
                    return (T)source.Data;
                }
            }
            catch (Exception)
            {
            }
            return default(T);
        }
    }

}
