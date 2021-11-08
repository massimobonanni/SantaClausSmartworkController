using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SCSC.PlatformFunctions.Entities;
using SCSC.PlatformFunctions.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCSC.PlatformFunctions.Services
{
    public class DefaultElfEntityFactory : IElfEntityFactory
    {
        public Task<EntityId> GetEntityIdAsync(string elfId, CancellationToken token)
        {
            return Task.FromResult(new EntityId(nameof(ElfSenior), elfId));
        }

        public Task<string> GetEntityNameAsync(string elfId, CancellationToken token)
        {
            return Task.FromResult(nameof(ElfSenior));
        }

        private static IEnumerable<string> EntityNames = new List<string>() { nameof(ElfSenior) };

        public Task<IEnumerable<string>> GetEntityNames(CancellationToken token)
        {
            return Task.FromResult(EntityNames);
        }
    }
}
