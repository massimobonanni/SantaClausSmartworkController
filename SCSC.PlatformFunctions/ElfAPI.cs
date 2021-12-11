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
using Newtonsoft.Json.Linq;
using SCSC.Core.Models;
using SCSC.PlatformFunctions.Entities.Interfaces;
using SCSC.PlatformFunctions.Filters;
using SCSC.PlatformFunctions.Services.Interfaces;
using SCSC.PlatformFunctions.Utilities;
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

        #region Open API Definition
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
        #endregion Open API Definition
        [FunctionName(nameof(PackageStarted))]
        public async Task<IActionResult> PackageStarted(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "elves/{elfId}/packagestarted")] HttpRequest req,
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

        #region Open API Definition
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
        #endregion Open API Definition
        [FunctionName(nameof(PackageEnded))]
        public async Task<IActionResult> PackageEnded(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "elves/{elfId}/packageended")] HttpRequest req,
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

        #region Open API Definition
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
        #endregion Open API Definition
        [FunctionName(nameof(CreateElf))]
        public async Task<IActionResult> CreateElf(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "elves")] HttpRequest req,
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

        #region Open API Definition
        [OpenApiOperation(operationId: "updateelf",
            Summary = "Update an existing elf in the platform.",
            Description = "Use this API to modify an existing elf in the platform.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(string),
            Summary = "Return the elf id modified",
            Description = "If the operation is succeeded, it return the elf id modified.")]
        [OpenApiParameter("elfId", Description = "Identifier of the elf to modify",
            Required = true, In = ParameterLocation.Path)]
        [OpenApiRequestBody(contentType: "json", bodyType: typeof(UpdateElfModel),
            Description = "Information about the elf to modify")]
        #endregion Open API Definition
        [FunctionName(nameof(UpdateElf))]
        public async Task<IActionResult> UpdateElf(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "elves/{elfId}")] HttpRequest req,
            string elfId,
            [DurableClient] IDurableEntityClient client,
            ILogger logger)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var elfModel = JsonConvert.DeserializeObject<UpdateElfModel>(requestBody);

            var entityId = await _entityfactory.GetEntityIdAsync(elfId, default);

            await client.SignalEntityAsync<IElfEntity>(entityId, d => d.Configure(elfModel.Configuration));

            return new OkObjectResult(elfId);
        }

        #region Open API Definition
        [OpenApiOperation(operationId: "deleteelf",
            Summary = "Remove an existing elf from the platform.",
            Description = "Use this API to remove an existing elf from the platform.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(string),
            Summary = "Return the elf id removed",
            Description = "If the operation is succeeded, it return the elf id deleted.")]
        [OpenApiParameter("elfId", Description = "Identifier of the elf to delete",
            Required = true, In = ParameterLocation.Path)]
        #endregion Open API Definition
        [FunctionName(nameof(DeleteElf))]
        public async Task<IActionResult> DeleteElf(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "elves/{elfId}")] HttpRequest req,
            string elfId,
            [DurableClient] IDurableEntityClient client,
            ILogger logger)
        {
            var entityId = await _entityfactory.GetEntityIdAsync(elfId, default);

            await client.SignalEntityAsync<IElfEntity>(entityId, d => d.Delete());

            return new OkObjectResult(elfId);
        }

        #region Open API Definition
        [OpenApiOperation(operationId: "getelves",
            Summary = "Get the list of elves.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(List<ElfInfoModel>),
            Summary = "The full list of elves")]
        [OpenApiParameter(name: "name",
            In = ParameterLocation.Query,
            Type = typeof(string),
            Summary = "Filter elves by name.",
            Description = "Retrieve only elves that have the value set in the name.")]
        #endregion Open API Definition
        [FunctionName(nameof(GetElves))]
        public async Task<IActionResult> GetElves(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "elves")] HttpRequest req,
           [DurableClient] IDurableEntityClient client,
           ILogger logger)
        {
            var result = new List<ElfInfoModel>();

            var filters = GetElvesFilters.CreateFromHttpRequest(req);

            EntityQuery queryDefinition = new EntityQuery()
            {
                PageSize = 100,
                FetchState = true,

            };

            var elfEntityNames = await this._entityfactory.GetEntityNamesAsync(default);

            do
            {
                EntityQueryResult queryResult = await client.ListEntitiesAsync(queryDefinition, default);

                foreach (var item in queryResult.Entities)
                {
                    if (elfEntityNames.Contains(item.EntityId.EntityName, new DurableEntityNameComparer()))
                    {
                        if (!item.IsDeleted())
                        {
                            ElfInfoModel elf = item.ToElfInfoModel();
                            if (filters.AreFiltersVerified(elf))
                                result.Add(elf);
                        }
                    }
                }

                queryDefinition.ContinuationToken = queryResult.ContinuationToken;
            } while (queryDefinition.ContinuationToken != null);

            return new OkObjectResult(result);
        }

        #region Open API Definition
        [OpenApiOperation(operationId: "getelf",
            Summary = "Retrieve information about a specifc elf.",
            Description = "Use this API to retrieve information for a specific elf identify by the id.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(ElfInfoModel),
            Summary = "Return the elf information",
            Description = "If the elf exists, the api returns the elf information.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound,
            contentType: "json",
            bodyType: typeof(string),
            Summary = "Return the elf id",
            Description = "If the elf don't exist, the api returns the elf id.")]
        [OpenApiParameter("elfId", Description = "Identifier of the elf to retrieve",
            Required = true, In = ParameterLocation.Path)]
        #endregion Open API Definition
        [FunctionName(nameof(GetElf))]
        public async Task<IActionResult> GetElf(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "elves/{elfId}")] HttpRequest req,
           string elfId,
           [DurableClient] IDurableEntityClient client,
           ILogger logger)
        {
            var elfEntityId = await this._entityfactory.GetEntityIdAsync(elfId, default);

            EntityStateResponse<JObject> entity = await client.ReadEntityStateAsync<JObject>(elfEntityId);
            if (entity.EntityExists)
            {
                var elf = entity.EntityState.ToElfInfoModel();
                elf.Id = elfId;
                return new OkObjectResult(elf);
            }
            return new NotFoundObjectResult(elfId);
        }

    }
}
