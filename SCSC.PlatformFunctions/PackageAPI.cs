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
    internal class PackageAPI
    {

        #region Open API Definition
        [OpenApiOperation(operationId: "getpackages",
            Summary = "Search completed packages in the history",
            Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK,
            contentType: "json",
            bodyType: typeof(List<PackageDetailModel>),
            Summary = "The full list of packages")]
        [OpenApiParameter(name: "elfname",
            In = ParameterLocation.Query,
            Type = typeof(string),
            Summary = "Filter packages by elf name",
            Description = "Retrieve the packages managed by the elf with the specific name.")]
        [OpenApiParameter(name: "from",
            In = ParameterLocation.Query,
            Type = typeof(string),
            Summary = "Filter packages by start time",
            Description = "Retrieve the packages managed after the date time (in the format 'yyyy-MM-ddTHH:mm:ss.fffffff+HH:mm') in the filter.")]
        [OpenApiParameter(name: "to",
            In = ParameterLocation.Query,
            Type = typeof(string),
            Summary = "Filter packages by end time",
            Description = "Retrieve the packages managed before the date time (in the format 'yyyy-MM-ddTHH:mm:ss.fffffff+HH:mm') in the filter.")]
        #endregion Open API Definition
        [FunctionName(nameof(GetPackages))]
        public async Task<IActionResult> GetPackages(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "packages")] HttpRequest req,
           [Table("packagesArchive", Connection = "PackagesStorageAccount")] CloudTable cloudTable,
           ILogger logger)
        {
            var result = new List<PackageDetailModel>();
            string filters = string.Empty;
            if (req.Query.ContainsKey("elfname"))
            {
                var condition = TableQuery.GenerateFilterCondition("ElfName", QueryComparisons.Equal, req.Query["elfname"]);
                if (string.IsNullOrEmpty(filters))
                    filters = condition;
                else
                    filters = TableQuery.CombineFilters(filters, TableOperators.And, condition);
            }

            if (req.Query.ContainsKey("from"))
            {
                var condition = TableQuery.GenerateFilterCondition("StartTimestamp", QueryComparisons.GreaterThan, req.Query["from"]);
                if (string.IsNullOrEmpty(filters))
                    filters = condition;
                else
                    filters = TableQuery.CombineFilters(filters, TableOperators.And, condition);
            }

            if (req.Query.ContainsKey("to"))
            {
                var condition = TableQuery.GenerateFilterCondition("StartTimestamp", QueryComparisons.LessThan, req.Query["to"]);
                if (string.IsNullOrEmpty(filters))
                    filters = condition;
                else
                    filters = TableQuery.CombineFilters(filters, TableOperators.And, condition);
            }

            TableQuery<ArchivePackage> rangeQuery = new TableQuery<ArchivePackage>().Where(filters);

            TableContinuationToken token = null;
            do
            {
                var segment = await cloudTable.ExecuteQuerySegmentedAsync(rangeQuery, token);
                token = segment.ContinuationToken;
                foreach (ArchivePackage package in segment)
                {
                    result.Add(package.ToDetailModel());
                }
            } while (token != null);

            return new OkObjectResult(result);
        }
    }
}
