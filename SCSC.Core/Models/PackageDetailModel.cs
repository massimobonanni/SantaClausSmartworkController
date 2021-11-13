using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCSC.Core.Models
{
    public class PackageDetailModel:PackageInfoModel
    {
        [JsonProperty("elfId")]
        public string ElfId { get; set; }
        [JsonProperty("elfName")]
        public string ElfName { get; set; }
    }
}
