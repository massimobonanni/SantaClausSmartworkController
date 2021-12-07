using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.PlatformFunctions.Entities.Interfaces
{
    public interface IElfEntity
    {
        void PackageStarted(PackageStartedModel package);
        void PackageEnded(PackageEndedModel package);
        void Configure(ElfConfigurationModel config);
        void Delete();
        Task<double?> GetHourProductivity();
        Task<DateTimeOffset?> GetLastUpdate();
    }
}
