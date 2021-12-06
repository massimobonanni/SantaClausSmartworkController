using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            {
                if (source.Data is string)
                    Data = source.Data.ToString();
                else if (source.Data is JObject)
                    Data = ((JObject)source.Data).ToString();
                else
                    Data = JsonConvert.SerializeObject(source.Data, Formatting.Indented);

                switch (Type)
                {
                    case AlertType.Productivity:
                        this.ProductivityAlertInfo = JsonConvert.DeserializeObject<ProductivityAlertInfoModel>(Data);
                        break;
                    case AlertType.Inactivity:
                        this.InactivityAlertInfo = JsonConvert.DeserializeObject<InactivityAlertInfoModel>(Data);
                        break;
                    default:
                        break;
                }
            }
        }

        public string Id { get; set; }
        public string AlertName { get; set; }
        public string ElfId { get; set; }
        public AlertType Type { get; set; }
        public AlertStatus Status { get; set; }
        public string Data { get; set; }
        public InactivityAlertInfoModel InactivityAlertInfo { get; set; } = new InactivityAlertInfoModel();

        public ProductivityAlertInfoModel ProductivityAlertInfo { get; set; } = new ProductivityAlertInfoModel();
    }
}
