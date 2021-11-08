using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.Core.Models
{
    public class ElfInfoModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("lastUpdate")]
        public DateTimeOffset LastUpdate { get; set; }

        [JsonProperty("LastPackages")]
        public List<PackageInfoModel> Packages { get; set; }
    }
}
