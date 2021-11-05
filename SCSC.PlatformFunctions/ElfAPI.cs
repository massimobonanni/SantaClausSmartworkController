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
using SCSC.PlatformFunctions.Entities.Interfaces;
using SCSC.PlatformFunctions.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SCSC.PlatformFunctions
{
    public class ElfAPI
    {
        private readonly IElfEntityFactory _entityfactory;

        public ElfAPI(IElfEntityFactory entityFactory)
        {
            _entityfactory = entityFactory;
        }

        [OpenApiOperation(operationId: "packagestarted",
            Summary = "A new packaging is started by an elf.",
            Description = "Use this API to signal that an elf starts to package a new gift for a kid.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(PackageStartedModel),
            Summary = "Packaging model used to signal the operation",
            Description = "Packaging model used to signal the operation")]
        [OpenApiParameter("elfId",Description ="Identifier of the elf that starts the package",
            Required =true,In =ParameterLocation.Path)]
        [OpenApiRequestBody(contentType:"json",bodyType:typeof(PackageStartedModel),Description ="Package information")]
        [FunctionName(nameof(PackagingStarted))]
        public async Task<IActionResult> PackagingStarted(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "elfs/{elfId}/packagestarted")] HttpRequest req,
            string elfId,
            [DurableClient] IDurableEntityClient client,
            ILogger logger)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var packagingModel = JsonConvert.DeserializeObject<PackageStartedModel>(requestBody);

            var entityId = await _entityfactory.GetEntityIdAsync(elfId, default);

            await client.SignalEntityAsync<IElfEntity>(entityId, d => d.PackageStarted(packagingModel));

            return new OkObjectResult(packagingModel);
        }

        [OpenApiOperation(operationId: "packageended",
            Summary = "A packaging is ended by an elf.",
            Description = "Use this API to signal that an elf ends to package a new gift for a kid.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(PackageEndedModel),
            Summary = "Packaging model used to signal the operation",
            Description = "Packaging model used to signal the operation")]
        [OpenApiParameter("elfId", Description = "Identifier of the elf that finishes the package",
            Required = true, In = ParameterLocation.Path)]
        [OpenApiRequestBody(contentType: "json", bodyType: typeof(PackageEndedModel), Description = "Package information")]
        [FunctionName(nameof(PackagingEnded))]
        public async Task<IActionResult> PackagingEnded(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "elfs/{elfId}/packageended")] HttpRequest req,
            string elfId,
            [DurableClient] IDurableEntityClient client,
            ILogger logger)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var packagingModel = JsonConvert.DeserializeObject<PackageEndedModel>(requestBody);

            var entityId = await _entityfactory.GetEntityIdAsync(elfId, default);

            await client.SignalEntityAsync<IElfEntity>(entityId, d => d.PackageEnded(packagingModel));

            return new OkObjectResult(packagingModel);
        }
    }
}
