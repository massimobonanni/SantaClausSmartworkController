using SCSC.Core.Models;
using System.Collections.Generic;

namespace SCSC.AdminWeb.Models.Alerts
{
    public class IndexViewModel
    {
        public string FilterElfId { get; set; }

        public IEnumerable<AlertInfoViewModel> Alerts { get; set; }
    }
}
