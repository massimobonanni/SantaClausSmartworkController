using DurableTask.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.PlatformFunctions
{
    internal class ManagementAPI
    {

        private readonly IConfiguration _configuration;

        public ManagementAPI(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        [FunctionName(nameof(PurgeHistory))]
        public async Task PurgeHistory(
            [TimerTrigger("%PurgeHistoryTimer%")] TimerInfo myTimer,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger logger)
        {
            logger.LogInformation($"[{nameof(PurgeHistory)}] --> START");

            var purgeRetention = this._configuration.GetValue<int>("PurgeRetentionInDays");
            var createdTimeTo = DateTime.Now.AddDays(-purgeRetention).Date;
            var purgeResponse = await client.PurgeInstanceHistoryAsync(DateTime.MinValue, createdTimeTo,
                new List<OrchestrationStatus>() { OrchestrationStatus.Terminated, OrchestrationStatus.Completed, OrchestrationStatus.Failed });
            
            logger.LogInformation($"[{nameof(PurgeHistory)}] --> Removed {purgeResponse.InstancesDeleted} instances created before {createdTimeTo}");
        }
    }


}
