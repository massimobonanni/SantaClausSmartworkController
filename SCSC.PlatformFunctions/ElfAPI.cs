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

        [OpenApiOperation(operationId: "packagingstarted",
            Summary = "A new packaging is started from an elf.",
            Description = "Use this API to signal that an elf starts to package a new gift for a kid.",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key",
            SecuritySchemeType.ApiKey,
            Name = "code",
            In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(PackagingOperationModel),
            Summary = "Packaging model used to signal the operation",
            Description = "Packaging model used to signal the operation")]
        [FunctionName(nameof(PackagingStarted))]
        public async Task<IActionResult> PackagingStarted(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "packaging/started")] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            ILogger logger)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var packagingModel = JsonConvert.DeserializeObject<PackagingOperationModel>(requestBody);

            var entityId = await _entityfactory.GetEntityIdAsync(packagingModel.ElfId, default);

            await client.SignalEntityAsync<IElfEntity>(entityId, d => d.PackagingStarted(packagingModel));

            return new OkObjectResult(packagingModel);
        }
    }
}
