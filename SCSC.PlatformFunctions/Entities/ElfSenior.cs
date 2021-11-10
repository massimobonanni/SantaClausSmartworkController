using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SCSC.Core.Models;
using SCSC.PlatformFunctions.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCSC.PlatformFunctions.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ElfSenior : IElfEntity
    {
        private readonly ILogger logger;

        public ElfSenior(ILogger logger)
        {
            this.logger = logger;
        }

        #region [ State ]
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("lastUpdate")]
        public DateTimeOffset LastUpdate { get; set; }

        [JsonProperty("LastPackages")]
        public List<PackageInfoModel> Packages { get; set; }= new List<PackageInfoModel>();
        #endregion [ State ]

        public void Configure(ElfConfigurationModel config)
        {
            throw new NotImplementedException();
        }

        public void PackageEnded(PackageEndedModel package)
        {
            throw new NotImplementedException();
        }

        public void PackageStarted(PackageStartedModel package)
        {
            throw new NotImplementedException();
        }


    }
}
