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
    }
}
