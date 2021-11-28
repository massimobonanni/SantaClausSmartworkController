using Microsoft.AspNetCore.Http;
using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCSC.PlatformFunctions.Filters
{
    public class GetAlertsFilters
    {
        public string ElfIdFilter { get; set; }



        public static GetAlertsFilters CreateFromHttpRequest(HttpRequest req)
        {
            if (req == null)
                throw new ArgumentNullException(nameof(req));

            var filter = new GetAlertsFilters();

            filter.ElfIdFilter = req.Query["elfId"];

            return filter;
        }

        public bool AreFiltersVerified(AlertInfoModel alert)
        {
            var result = true;

            if (!string.IsNullOrEmpty(this.ElfIdFilter))
            {
                result &= alert.ElfId == this.ElfIdFilter;
            }

            return result;
        }
    }
}
