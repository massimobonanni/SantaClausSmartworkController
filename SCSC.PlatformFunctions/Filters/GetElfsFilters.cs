using Microsoft.AspNetCore.Http;
using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SCSC.PlatformFunctions.Filters
{
    public class GetElfsFilters
    {
        public string ElfNameFilter { get; set; }

        

        public static GetElfsFilters CreateFromHttpRequest(HttpRequest req)
        {
            if (req == null)
                throw new ArgumentNullException(nameof(req));

            var filter = new GetElfsFilters();

            filter.ElfNameFilter = req.Query["name"];

            return filter;
        }

        public bool AreFiltersVerified(ElfInfoModel elf)
        {
            var result = true;

            if (!string.IsNullOrEmpty(this.ElfNameFilter))
            {
                result &= elf.Name.Contains(this.ElfNameFilter, StringComparison.OrdinalIgnoreCase);
            }
            
            return result;
        }
    }
}
