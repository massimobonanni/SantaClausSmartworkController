using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SCSC.Core.Models;
using SCSC.PlatformFunctions.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCSC.PlatformFunctions
{
    internal class AlertAPI
    {
        private readonly IAlertOrchestratorFactory _orchestratorfactory;

        public AlertAPI(IAlertOrchestratorFactory orchestratorfactory)
        {
            _orchestratorfactory = orchestratorfactory;
        }

        //  API to implements
        //      api/alerts                  --> [GET] retrieve all the alert running
        //      api/alerts/{alertId}        --> [GET] retrieve a specific alert
        //      api/alerts/{alertId}/cancel --> [POST] cancel an alert
        //      api/alerts                  --> [POST] create an alert

        #region Open API Definition
        [OpenApiOperation(operationId: "getalerts",
            Summary = "Get the list of alerts.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(List<AlertInfoModel>),
            Summary = "The full list of alerts")]
        #endregion Open API Definition
        [FunctionName(nameof(GetAlerts))]
        public async Task<IActionResult> GetAlerts(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "alerts")] HttpRequest req,
           [DurableClient] IDurableOrchestrationClient client,
           ILogger logger)
        {
            var result = new List<AlertInfoModel>();
            var noFilter = new OrchestrationStatusQueryCondition()
            {
                PageSize = 100,
                ShowInput = true,
                RuntimeStatus = new List<OrchestrationRuntimeStatus> { OrchestrationRuntimeStatus.Running }
            };

            do
            {
                OrchestrationStatusQueryResult queryResult = await client.ListInstancesAsync(
                    noFilter,
                    CancellationToken.None);

                foreach (var item in queryResult.DurableOrchestrationState)
                {
                    result.Add(item.ToAlertInfoModel());
                }

                noFilter.ContinuationToken = queryResult.ContinuationToken;
            } while (noFilter.ContinuationToken != null);

            return new OkObjectResult(result);
        }
    }
}
