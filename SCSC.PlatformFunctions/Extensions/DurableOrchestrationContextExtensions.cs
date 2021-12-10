using Dynamitey;
using SCSC.Core.Models;
using SCSC.PlatformFunctions.Activities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.WebJobs.Extensions.DurableTask
{
    internal static class DurableOrchestrationContextExtensions
    {

        public static async Task SendAlertSMSAsync(this IDurableOrchestrationContext context,
            string smsFrom,string smsToNotify,string extendedMessage = null)
        {
            if (context == null)
                throw new NullReferenceException(nameof(context));

            var createAlertInfo = context.GetInput<CreateAlertModel>();

            var notification = new AlertNotificationActivity.SmsInfoModel();
            var message = new StringBuilder();
            message.Append($"The alert {createAlertInfo.AlertName} is fired for the elf {createAlertInfo.ElfId} at {context.CurrentUtcDateTime}.");
            if (extendedMessage != null)
                message.Append($" {extendedMessage}");
            notification.Message = message.ToString();
            notification.FromPhoneNumber = smsFrom;
            notification.ToPhoneNumber = smsToNotify;
            await context.CallActivityAsync(nameof(AlertNotificationActivity.SendSMS), notification);
        }

        public static async Task SendAlertEmailAsync(this IDurableOrchestrationContext context,
            string emailFrom, string emailToNotify, string extendedMessage = null)
        {
            if (context == null)
                throw new NullReferenceException(nameof(context));

            var createAlertInfo = context.GetInput<CreateAlertModel>();

            var notification = new AlertNotificationActivity.EmailInfoModel();
            notification.From = emailFrom;
            notification.Subject = $"Alert for elf {createAlertInfo.ElfId}";
            var body = new StringBuilder();
            body.Append($"The alert {createAlertInfo.AlertName} is fired for the elf {createAlertInfo.ElfId} at {context.CurrentUtcDateTime}.");
            if (extendedMessage != null)
                body.Append($" {extendedMessage}");
            notification.Body = body.ToString();
            notification.Tos = new List<string>() { emailToNotify };
            await context.CallActivityAsync(nameof(AlertNotificationActivity.SendEmail), notification);
        }
    }
}
