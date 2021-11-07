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
        [OpenApiParameter("elfId", Description = "Identifier of the elf that starts the package",
            Required = true, In = ParameterLocation.Path)]
        [OpenApiRequestBody(contentType: "json", bodyType: typeof(PackageStartedModel), Description = "Package information")]
        [FunctionName(nameof(PackageStarted))]
        public async Task<IActionResult> PackageStarted(
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
        [FunctionName(nameof(PackageEnded))]
        public async Task<IActionResult> PackageEnded(
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

        [OpenApiOperation(operationId: "createelf",
            Summary = "Add a new elf in the platform.",
            Description = "Use this API to create a new elf in the platform.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest,
            bodyType: typeof(string),
            contentType: "json",
            Description = "You receive and error message if the elf id is empty or composed by only spaces",
            Summary = "Return bad request if the elf id for the creation is not valid")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(string),
            Summary = "Return the elf id created",
            Description = "If the operation is succeeded, it return the elf id created.")]
        [OpenApiRequestBody(contentType: "json", bodyType: typeof(CreateElfModel), Description = "Information about the elf to create")]
        [FunctionName(nameof(CreateElf))]
        public async Task<IActionResult> CreateElf(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "elfs")] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            ILogger logger)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var elfModel = JsonConvert.DeserializeObject<CreateElfModel>(requestBody);

            if (string.IsNullOrWhiteSpace(elfModel.ElfId))
                return new BadRequestObjectResult("Elf ID is not valid");

            var entityId = await _entityfactory.GetEntityIdAsync(elfModel.ElfId, default);

            await client.SignalEntityAsync<IElfEntity>(entityId, d => d.Configure(elfModel.Configuration));

            return new OkObjectResult(elfModel.ElfId);
        }
    }
}
