using SCSC.Core.Models;
using System.Collections.Generic;

namespace SCSC.AdminWeb.Models.Elves
{
    public class IndexViewModel
    {
        public string FilterName { get; set; }

        public IEnumerable<ElfInfoViewModel> Elves { get; set; }
    }
}
