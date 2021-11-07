using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.Core.Models
{
    public class CreateElfModel
    {
        [JsonProperty("configuration")]
        public ElfConfigurationModel Configuration { get; set; }
        [JsonProperty("elfId")]
        public string ElfId { get; set; }
    }
}
