using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCSC.Core.Models
{
    public class PackageInfoModel
    {
        [JsonProperty("startTimestamp")]
        public DateTimeOffset StartTimestamp { get; set; }
        [JsonProperty("endTimestamp")]
        public DateTimeOffset? EndTimestamp { get; set; }
        [JsonProperty("packageId")]
        public string PackageId { get; set; }
        [JsonProperty("giftDescription")]
        public string GiftDescription { get; set; }
        [JsonProperty("kidName")]
        public string KidName { get; set; }

        public double? DurationInSec
        {
            get
            { 
                if (!this.EndTimestamp.HasValue)
                    return null;

                return this.EndTimestamp.Value.Subtract(this.StartTimestamp).TotalSeconds;
            }
        }
    }
}
