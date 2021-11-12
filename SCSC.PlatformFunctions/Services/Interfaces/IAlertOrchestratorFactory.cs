using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCSC.PlatformFunctions.Services.Interfaces
{
    public interface IAlertOrchestratorFactory
    {
        Task<string> GetOrchestratorNameAsync(AlertType alertType, CancellationToken token);
    }
}
