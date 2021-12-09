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
using SCSC.PlatformFunctions.Activities;

namespace SCSC.PlatformFunctions.Orchestrators
{
    internal class ProductivityAlertOrchestrator
    {
        private readonly IElfEntityFactory _entityfactory;
        private readonly IConfiguration _configuration;

        public ProductivityAlertOrchestrator(IElfEntityFactory entityFactory, IConfiguration configuration)
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
        ///         "productivityPerHourThreshold" : 5, --> the alert will be throw if the productivity of the elf will go under the 5 packs per hour
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

            [JsonProperty("productivityPerHourThreshold")]
            public double ProductivityPerHourThreshold { get; set; }

            [JsonProperty("pollingTimeInSec")]
            public int PollingTimeInSec { get; set; } = 30;

        }


        [FunctionName(nameof(ProductivityAlert))]
        public async Task ProductivityAlert(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            var createAlertInfo = context.GetInput<CreateAlertModel>();
            context.SetCustomStatus(createAlertInfo);
            logger.LogInformation($"[START ORCHESTRATOR] --> {nameof(ProductivityAlertOrchestrator.ProductivityAlert)} {createAlertInfo.AlertName} for elf {createAlertInfo.ElfId}");

            var alertInfo = createAlertInfo.ExtractFromData<AlertInfo>();

            var elfEntityId = await this._entityfactory.GetEntityIdAsync(createAlertInfo.ElfId, CancellationToken.None);

            var numberOfIteration = alertInfo.DurationInSec / alertInfo.PollingTimeInSec;

            for (int i = 0; i < numberOfIteration; i++)
            {
                var timeoutFireAt = context.CurrentUtcDateTime.AddSeconds(alertInfo.PollingTimeInSec);
                await context.CreateTimer(timeoutFireAt, CancellationToken.None);

                var currentProductivity = await context.CallEntityAsync<double?>(elfEntityId, nameof(IElfEntity.GetHourProductivity));
                if (currentProductivity.HasValue && currentProductivity.Value < alertInfo.ProductivityPerHourThreshold)
                {
                    logger.LogInformation($"Productivity threshold reached for elf {createAlertInfo.ElfId}", createAlertInfo);
                    if (!string.IsNullOrWhiteSpace(alertInfo.SMSToNotify))
                    {
                        await context.SendAlertSMSAsync(this._configuration.GetValue<string>("TwilioFromNumber"),
                                alertInfo.SMSToNotify, $"The elf {createAlertInfo.ElfId} went down {alertInfo.ProductivityPerHourThreshold} packages/hour");
                    }
                    if (!string.IsNullOrWhiteSpace(alertInfo.EmailToNotify))
                    {
                        await context.SendAlertEmailAsync(this._configuration.GetValue<string>("EmailNotificationFrom"),
                               alertInfo.EmailToNotify,
                               $"The elf {createAlertInfo.ElfId} had a productivity {currentProductivity} packages/hour, less than {alertInfo.ProductivityPerHourThreshold} packages/hour");
                    }

                    await context.CallActivityAsync(nameof(AlertNotificationActivity.SaveAlertNotification),
                            (context.InstanceId, createAlertInfo));

                    break;
                }
            }

        }
    }
}