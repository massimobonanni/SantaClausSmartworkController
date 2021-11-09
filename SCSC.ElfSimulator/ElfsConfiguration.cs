using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SCSC.ElfSimulator
{
    public class ElfsConfiguration
    {
        [JsonPropertyName("elfs")]
        public IEnumerable<ElfConfiguration> Elfs { get; set; }

        [JsonPropertyName("apiBaseUrl")]
        public string ApiBaseUrl { get; set; }
    }

    public class ElfConfiguration
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("operationDurationMin")]
        public int OperationDurationMinInSec { get; set; }

        [JsonPropertyName("operationDurationMax")]
        public int OperationDurationMaxInSec { get; set; }

        [JsonPropertyName("breakDuration")]
        public int BreakDurationMaxInSec { get; set; } = 30;

    }
}