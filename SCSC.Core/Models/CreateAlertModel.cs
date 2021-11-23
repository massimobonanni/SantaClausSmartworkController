using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCSC.Core.Models
{
    public class CreateAlertModel
    {
        [JsonProperty("elfId")]
        public string ElfId { get; set; }

        [JsonProperty("alertName")]
        public string AlertName { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public AlertType Type { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
