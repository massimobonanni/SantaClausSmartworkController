using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.Core.Models
{
    public class PackagingOperationModel
    {
        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
        [JsonProperty("elfId")]
        public string ElfId { get; set; }
        [JsonProperty("elfName")]
        public string ElfName { get; set; }
        [JsonProperty("package")]
        public PackageModel Package { get; set; }
    }
}
