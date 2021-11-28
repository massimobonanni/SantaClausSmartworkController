using Newtonsoft.Json;
using SCSC.Core.Models;
using System;
using System.Collections.Generic;

namespace SCSC.AdminWeb.Models.Alerts
{
    public class AlertInfoViewModel
    {
        public AlertInfoViewModel()
        {

        }

        public AlertInfoViewModel(AlertInfoModel source)
        {
            Id = source.Id;
            AlertName = source.AlertName;
            ElfId = source.ElfId;
            Type = source.Type;
            Status = source.Status;
            if (source.Data != null)
                Data = JsonConvert.SerializeObject(source.Data, Formatting.Indented);
        }

        public string Id { get; set; }
        public string AlertName { get; set; }
        public string ElfId { get; set; }
        public AlertType Type { get; set; }
        public AlertStatus Status { get; set; }
        public string Data { get; set; }

    }
}
