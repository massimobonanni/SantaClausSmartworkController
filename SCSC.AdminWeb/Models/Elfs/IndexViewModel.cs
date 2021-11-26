using SCSC.Core.Models;
using System.Collections.Generic;

namespace SCSC.AdminWeb.Models.Elfs
{
    public class IndexViewModel
    {
        public string FilterName { get; set; }

        public IEnumerable<ElfInfoViewModel> Elfs { get; set; }
    }
}
