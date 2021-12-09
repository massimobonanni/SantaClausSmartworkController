using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SCSC.Core.Models;
using SCSC.PlatformFunctions.Filters;
using SCSC.PlatformFunctions.Orchestrators;
using SCSC.PlatformFunctions.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        //      api/alerts/{alertId}/cancel --> [POST] cancel an alert

        #region Open API Definition
        [OpenApiOperation(operationId: "getalerts",
            Summary = "Get the list of alerts in running state.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(List<AlertInfoModel>),
            Summary = "The full list of alerts")]
        [OpenApiParameter(name: "elfId",
            In = ParameterLocation.Query,
            Type = typeof(string),
            Summary = "Filter alerts by elf id.",
            Description = "Retrieve only alerts related ta specific elf by elfid.")]
        #endregion Open API Definition
        [FunctionName(nameof(GetAlerts))]
        public async Task<IActionResult> GetAlerts(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "alerts")] HttpRequest req,
           [DurableClient] IDurableOrchestrationClient client,
           ILogger logger)
        {
            var result = new List<AlertInfoModel>();

            var filters = GetAlertsFilters.CreateFromHttpRequest(req);
            var orchestratorNames = _orchestratorfactory.GetOrchestratorNames();

            var noFilter = new OrchestrationStatusQueryCondition()
            {
                PageSize = 100,
                ShowInput = true,
                InstanceIdPrefix = GlobalConstants.AlertPrefixInstanceId,
                RuntimeStatus = new List<OrchestrationRuntimeStatus> { OrchestrationRuntimeStatus.Running,
                    OrchestrationRuntimeStatus.Completed }
            };

            do
            {
                OrchestrationStatusQueryResult queryResult = await client.ListInstancesAsync(
                    noFilter,
                    CancellationToken.None);

                foreach (var item in queryResult.DurableOrchestrationState)
                {
                    if (orchestratorNames.Contains(item.Name))
                    {
                        var alert = item.ToAlertInfoModel();
                        if (filters.AreFiltersVerified(alert))
                            result.Add(alert);
                    }
                }

                noFilter.ContinuationToken = queryResult.ContinuationToken;
            } while (noFilter.ContinuationToken != null);

            return new OkObjectResult(result);
        }

        #region Open API Definition
        [OpenApiOperation(operationId: "getalert",
            Summary = "Get the informatiojn about one specific alert.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(AlertInfoModel),
            Summary = "The information of the alert")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound,
            contentType: "json",
            bodyType: typeof(string),
            Summary = "If the alert doesn't exist")]
        #endregion Open API Definition
        [FunctionName(nameof(GetAlert))]
        public async Task<IActionResult> GetAlert(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "alerts/{alertId}")] HttpRequest req,
           string alertId,
           [DurableClient] IDurableOrchestrationClient client,
           ILogger logger)
        {
            var orchestrator = await client.GetStatusAsync(alertId);
            if (orchestrator != null)
            {
                return new OkObjectResult(orchestrator.ToAlertInfoModel());
            }

            return new NotFoundObjectResult("The alert doesn't exist");
        }

        #region Open API Definition
        [OpenApiOperation(operationId: "createalert",
            Summary = "Create a new alert for an elf.",
            Description = "Use this API to create a new alert for a specific elf.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest,
            bodyType: typeof(string),
            contentType: "json",
            Description = "You receive an error message if the body of the request is not valid",
            Summary = "Return bad request if the body request is not valid")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(string),
            Summary = "Return the alert id created",
            Description = "If the operation is succeeded, it return the alert id created.")]
        [OpenApiRequestBody(contentType: "json", bodyType: typeof(CreateAlertModel), Description = "Information about the alert to create")]
        #endregion Open API Definition
        [FunctionName(nameof(CreateAlert))]
        public async Task<IActionResult> CreateAlert(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "alerts")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger logger)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var alertModel = JsonConvert.DeserializeObject<CreateAlertModel>(requestBody);

            if (string.IsNullOrWhiteSpace(alertModel.ElfId))
                return new BadRequestObjectResult("Elf ID is not valid");

            var orchestratorName = await _orchestratorfactory.GetOrchestratorNameAsync(alertModel.Type, default);
            alertModel.CreationTimeStamp = DateTimeOffset.UtcNow;
            var instanceId = $"{GlobalConstants.AlertPrefixInstanceId}{Guid.NewGuid()}";
            var orchestratorId = await client.StartNewAsync<CreateAlertModel>(orchestratorName, instanceId, alertModel);

            return new OkObjectResult(orchestratorId);
        }

        #region Open API Definition
        [OpenApiOperation(operationId: "cancelalert",
            Summary = "Cancel an alert for an elf.",
            Description = "Use this API to cancel a running alert for a specific elf.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(string),
            Summary = "Return the alert id cancelled",
            Description = "If the operation is succeeded, it return the alert id cancelled.")]
        [OpenApiRequestBody(contentType: "json", bodyType: typeof(CreateAlertModel), Description = "Information about the alert to create")]
        #endregion Open API Definition
        [FunctionName(nameof(CancelAlert))]
        public async Task<IActionResult> CancelAlert(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "alerts/{alertId}/cancel")] HttpRequest req,
            string alertId,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger logger)
        {
            await client.RaiseEventAsync(alertId, AlertOrchestratorEvents.Cancel);

            return new OkObjectResult(alertId);
        }
    }
}
