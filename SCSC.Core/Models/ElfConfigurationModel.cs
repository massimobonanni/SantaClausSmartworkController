using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.Core.Models
{
    public class ElfConfigurationModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("startWorkTime")]
        public string StartWorkTime { get; set; } = "09:00:00";

        [JsonProperty("endWorkTime")]
        public string EndWorkTime { get; set; } = "18:00:00";
    }
}
