using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SCSC.Core.Models;
using SCSC.PlatformFunctions.Entities;
using SCSC.PlatformFunctions.Orchestrators;
using SCSC.PlatformFunctions.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCSC.PlatformFunctions.Services
{
    public class DefaultAlertOrchestratorFactory : IAlertOrchestratorFactory
    {
        public Task<string> GetOrchestratorNameAsync(AlertType alertType, CancellationToken token)
        {
            switch (alertType)
            {
                case AlertType.Productivity:
                    return Task.FromResult(nameof(ProductivityAlertOrchestrator.ProductivityAlert));
                case AlertType.Inactivity:
                    return Task.FromResult(nameof(InactivityAlertOrchestrator.InactivityAlert));
                default:
                    throw new ArgumentException(nameof(alertType));
            }
        }
    }
}
