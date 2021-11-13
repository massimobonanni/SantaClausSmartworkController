using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SCSC.Core.Models;
using SCSC.PlatformFunctions.Entities.Interfaces;
using SCSC.PlatformFunctions.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCSC.PlatformFunctions.Orchestrators
{
    internal class InactivityAlertOrchestrator
    {
        private readonly IElfEntityFactory _entityfactory;
        private readonly IConfiguration _configuration;

        public InactivityAlertOrchestrator(IElfEntityFactory entityFactory, IConfiguration configuration)
        {
            _entityfactory = entityFactory;
            _configuration = configuration;
        }

        /// <example>
        /// Info sample
        /// <code>
        ///     {
        ///         "durationInSec": 600, --> the alert runs for 10 minutes 
        ///         "smsToNotify" : "+1555897865", 
        ///         "maxInactivityTimeInMinutes" : 5, --> the alert will be throw if the elf didn't work for 5 minutes
        ///         "pollingTimeInSec" : 30 --> check the threshold every 30 seconds
        ///     }
        ///</code>
        /// </example>
        public class AlertInfo
        {
            [JsonProperty("durationInSec")]
            public int DurationInSec { get; set; }

            [JsonProperty("smsToNotify")]
            public string SMSToNotify { get; set; }

            [JsonProperty("emailToNotify")]
            public string EmailToNotify { get; set; }

            [JsonProperty("maxInactivityTimeInMinutes")]
            public int MaxInactivityTimeInMinutes { get; set; }

            [JsonProperty("pollingTimeInSec")]
            public int PollingTimeInSec { get; set; } = 30;

        }


        [FunctionName(nameof(InactivityAlert))]
        public async Task InactivityAlert(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            logger.LogInformation($"[START ORCHESTRATOR] --> {nameof(InactivityAlertOrchestrator.InactivityAlert)}");
            var createAlertInfo = context.GetInput<CreateAlertModel>();
            context.SetCustomStatus(createAlertInfo);

            var alertInfo = createAlertInfo.ExtractFromData<AlertInfo>();

            var elfEntityId = await this._entityfactory.GetEntityIdAsync(createAlertInfo.ElfId, CancellationToken.None);

            var startTime = context.CurrentUtcDateTime;
            var cancelEvent = context.WaitForExternalEvent(AlertOrchestratorEvents.Cancel);

            while (!cancelEvent.IsCompleted && context.CurrentUtcDateTime <= startTime.AddSeconds(alertInfo.DurationInSec))
            {
                var currentProductivity = await context.CallEntityAsync<double>(elfEntityId, nameof(IElfEntity.GetHourProductivity));
                // add logic here
                await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(alertInfo.PollingTimeInSec), CancellationToken.None);
            }
        }
    }
}
