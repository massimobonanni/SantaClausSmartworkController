using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.Core.Models
{
    public class PackageStartedModel
    {
        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
        [JsonProperty("packageId")]
        public string PackageId { get; set; }
        [JsonProperty("giftDescription")]
        public string GiftDescription { get; set; }
        [JsonProperty("kidName")]
        public string KidName { get; set; }
    }
}
