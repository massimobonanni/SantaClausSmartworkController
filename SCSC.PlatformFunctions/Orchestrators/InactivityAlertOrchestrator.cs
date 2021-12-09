using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SCSC.Core.Models;
using SCSC.PlatformFunctions.Activities;
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
            var createAlertInfo = context.GetInput<CreateAlertModel>();
            context.SetCustomStatus(createAlertInfo);
            logger.LogInformation($"[START ORCHESTRATOR] --> {nameof(InactivityAlertOrchestrator.InactivityAlert)} {createAlertInfo.AlertName} for elf {createAlertInfo.ElfId}");

            var alertInfo = createAlertInfo.ExtractFromData<AlertInfo>();

            var elfEntityId = await this._entityfactory.GetEntityIdAsync(createAlertInfo.ElfId, CancellationToken.None);

            var numberOfIteration = alertInfo.DurationInSec / alertInfo.PollingTimeInSec;

            for (int i = 0; i < numberOfIteration; i++)
            {
                var timeoutFireAt = context.CurrentUtcDateTime.AddSeconds(alertInfo.PollingTimeInSec);
                await context.CreateTimer(timeoutFireAt, CancellationToken.None);

                var lastUpdate = await context.CallEntityAsync<DateTimeOffset?>(elfEntityId, nameof(IElfEntity.GetLastUpdate));
                if (lastUpdate.HasValue)
                {
                    var inactivityMinutes = DateTimeOffset.Now.Subtract(lastUpdate.Value).TotalMinutes;
                    if (alertInfo.MaxInactivityTimeInMinutes < inactivityMinutes)
                    {
                        logger.LogInformation($"Productivity threshold reached for elf {createAlertInfo.ElfId}", createAlertInfo);

                        if (!string.IsNullOrWhiteSpace(alertInfo.SMSToNotify))
                        {
                            await context.SendAlertSMSAsync(this._configuration.GetValue<string>("TwilioFromNumber"),
                                alertInfo.SMSToNotify, $"The elf {createAlertInfo.ElfId} is inactive since {Math.Round(inactivityMinutes)} minutes");
                        }
                        if (!string.IsNullOrWhiteSpace(alertInfo.EmailToNotify))
                        {
                            await context.SendAlertEmailAsync(this._configuration.GetValue<string>("EmailNotificationFrom"),
                                alertInfo.EmailToNotify, $"The elf {createAlertInfo.ElfId} is inactive since {Math.Round(inactivityMinutes)} minutes, more than {alertInfo.MaxInactivityTimeInMinutes} minutes");
                        }

                        await context.CallActivityAsync(nameof(AlertNotificationActivity.SaveAlertNotification),
                            (context.InstanceId, createAlertInfo));

                        break;
                    }
                }
            }
        }

    }
}
