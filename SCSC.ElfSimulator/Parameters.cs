using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SCSC.ElfSimulator
{
    internal class Parameters
    {
        [Option(
            'f',
            "ConfigFile",
            HelpText = "JSON File contains the elves configuration")]
        public string ConfigFilePath { get; set; }

        [Option(
            'b',
            "BlobUrl",
            HelpText = "Url for the blob contains JSON configuration for the elves")]
        public string BlobUrl { get; set; }

        public bool IsValid()
        {
            return !(string.IsNullOrWhiteSpace(BlobUrl) || string.IsNullOrWhiteSpace(ConfigFilePath));
        }

        public bool IsFileConfig()
        {
            return !string.IsNullOrWhiteSpace(ConfigFilePath);
        }
    }
}
