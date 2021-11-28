using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.Core.Models
{
    public class AlertInfoModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("elfId")]
        public string ElfId { get; set; }
        
        [JsonProperty("alertName")]
        public string AlertName { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AlertType Type { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AlertStatus Status { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
