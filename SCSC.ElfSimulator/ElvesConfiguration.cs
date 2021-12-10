using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SCSC.ElfSimulator
{
    public class ElvesConfiguration
    {
        [JsonPropertyName("elves")]
        public IEnumerable<ElfConfiguration> Elves { get; set; }

        [JsonPropertyName("apiBaseUrl")]
        public string ApiBaseUrl { get; set; }
        
        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; }
    }

    public class ElfConfiguration
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("operationDurationMin")]
        public int OperationDurationMinInSec { get; set; } = 60;

        [JsonPropertyName("operationDurationMax")]
        public int OperationDurationMaxInSec { get; set; } = 300;

        [JsonPropertyName("breakDuration")]
        public int BreakDurationMaxInSec { get; set; } = 30;

        [JsonPropertyName("startWorkTime")]
        public string StartWorkTime { get; set; } = "09:00:00";

        [JsonPropertyName("endWorkTime")]
        public string EndWorkTime { get; set; } = "18:00:00";

        [JsonPropertyName("lazyPercentage")]
        public double LazyPercentage { get; set; } = 0.0;
    }
}