using Microsoft.Azure.WebJobs.Extensions.DurableTask;
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
            throw new NotImplementedException();
        }
    }
}
