using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCSC.PlatformFunctions.Services.Interfaces
{
    public interface IElfEntityFactory
    {
        Task<EntityId> GetEntityIdAsync(string elfId, CancellationToken token);
        Task<string> GetEntityNameAsync(string elfId, CancellationToken token);

        Task<IEnumerable<string>> GetEntityNamesAsync(CancellationToken token);
    }
}
