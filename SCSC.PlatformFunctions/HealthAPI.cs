using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using SCSC.Core.Models;
using SCSC.PlatformFunctions.Orchestrators;
using SCSC.PlatformFunctions.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static SCSC.PlatformFunctions.Orchestrators.PackageArchiverOrchestrator;

namespace SCSC.PlatformFunctions
{
    internal class HealthAPI
    {

        #region Open API Definition
        [OpenApiOperation(operationId: "gethealth",
            Summary = "Return HTTP 200 if the function works",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "text",
            bodyType: typeof(string),
            Summary = "The string 'OK'")]
        #endregion Open API Definition
        [FunctionName(nameof(GetHealth))]
        public IActionResult GetHealth(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest req,
           ILogger logger)
        {
            logger.LogTrace($"{nameof(GetHealth)} called from {req.HttpContext?.Connection?.RemoteIpAddress}");
            return new OkObjectResult("OK");
        }
    }
}
