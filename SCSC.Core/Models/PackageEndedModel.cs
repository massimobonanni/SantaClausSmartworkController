using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.Core.Models
{
    public class PackageEndedModel
    {
        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
        [JsonProperty("packageId")]
        public string PackageId { get; set; }
    }
}
