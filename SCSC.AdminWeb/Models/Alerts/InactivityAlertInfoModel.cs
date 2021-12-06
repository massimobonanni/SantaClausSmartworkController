using System;
using System.ComponentModel.DataAnnotations;

namespace SCSC.AdminWeb.Models.Alerts
{
    public class InactivityAlertInfoModel
    {
        [Display(Name = "Alert duretion (in secs)", Description = "The duration in secs of the alert")]
        public int DurationInSec { get; set; }

        [Display(Name = "SMS to notify the alert", Description = "The sms to use to notify message when the alert fires")]
        public string SMSToNotify { get; set; }

        [Display(Name = "Email to notify the alert", Description = "The email to use to notify message when the alert fires")]
        public string EmailToNotify { get; set; }

        [Display(Name = "Max inactivity time (in minutes)", Description = "The threshold of the alert: the alert will be fired when the elf doesn't work for that amount of minutes")]
        public int MaxInactivityTimeInMinutes { get; set; }

        [Display(Name = "Polling time (in secs)", Description = "The polling time for the alert check")]
        public int PollingTimeInSec { get; set; } = 30;

        public bool IsValid()
        {
            var isValid = true;

            isValid &= MaxInactivityTimeInMinutes > 0;
            isValid &= PollingTimeInSec > 0;
            isValid &= DurationInSec > 0;

            return isValid;
        }
    }
}
