using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml;

namespace SCSC.AdminWeb.Models.Alerts
{
    public class CreateViewModel
    {
        public CreateViewModel()
        {

        }

        //public CreateViewModel(AlertInfoModel source)
        //{
        //    Id = source.Id;
        //    AlertName = source.AlertName;
        //    ElfId = source.ElfId;
        //    Type = source.Type;
        //    if (source.Data != null)
        //    {
        //        var data = JsonConvert.SerializeObject(source.Data);
        //        switch (source.Type)
        //        {
        //            case AlertType.Productivity:
        //                ProductivityAlertInfo = JsonConvert.DeserializeObject<ProductivityAlertInfoModel>(data);
        //                break;
        //            case AlertType.Inactivity:
        //                InactivityAlertInfo = JsonConvert.DeserializeObject<InactivityAlertInfoModel>(data);
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //}

        [Required(ErrorMessage = "Alert name is mandatory")]
        [Display(Name = "Alert Name", Description = "The name of the alert")]
        public string AlertName { get; set; }

        [Required(ErrorMessage = "Elf Id is mandatory")]
        [Display(Name = "Elf ID", Description = "The id of the elf you want to set the alert")]
        public string ElfId { get; set; }

        public List<SelectListItem> ElfIds { get; set; }

        [Display(Name = "Alert Type", Description = "The type of the alert")]
        public AlertType Type { get; set; }

        public List<SelectListItem> AlertTypes { get; } = Utilities.SelectList.Create<AlertType>();

        public InactivityAlertInfoModel InactivityAlertInfo { get; set; } = new InactivityAlertInfoModel();

        public ProductivityAlertInfoModel ProductivityAlertInfo { get; set; } = new ProductivityAlertInfoModel();


        public CreateAlertModel ToCoreAlert()
        {
            var alert = new CreateAlertModel()
            {
                AlertName = this.AlertName,
                ElfId = this.ElfId,
                Type = this.Type
            };

            switch (this.Type)
            {
                case AlertType.Inactivity:
                    alert.Data = JsonConvert.SerializeObject(this.InactivityAlertInfo);
                    break;
                case AlertType.Productivity:
                    alert.Data = JsonConvert.SerializeObject(this.ProductivityAlertInfo);
                    break;
                default:
                    break;
            }

            return alert;
        }

        public bool IsValid()
        {
            var isValid = true;
            isValid &= !string.IsNullOrWhiteSpace(this.AlertName);
            isValid &= !string.IsNullOrWhiteSpace(this.ElfId);
            switch (this.Type)
            {
                case AlertType.Inactivity:
                    isValid &= this.InactivityAlertInfo.IsValid();
                    break;
                case AlertType.Productivity:
                    isValid &= this.ProductivityAlertInfo.IsValid();
                    break;
                default:
                    isValid = false;
                    break;
            }
            return isValid;
        }
    }
}
