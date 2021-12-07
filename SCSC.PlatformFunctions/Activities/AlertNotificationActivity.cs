using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SCSC.Core.Models;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SCSC.PlatformFunctions.Activities
{
    internal class AlertNotificationActivity
    {
        private readonly IConfiguration configuration;

        public AlertNotificationActivity(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public class EmailInfoModel
        {
            public string From { get; set; }
            public List<string> Tos { get; set; }
            public List<string> CCs { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
        }

        [FunctionName(nameof(SendEmail))]
        public async Task SendEmail([ActivityTrigger] EmailInfoModel notification,
            [SendGrid(ApiKey = "SendGridApiKey")] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log)
        {
            log.LogInformation($"[START ACTIVITY] --> {nameof(SendEmail)}");

            var message = new SendGridMessage()
            {
                Subject = notification.Subject,
                From = new EmailAddress(notification.From)
            };
            if (notification.Tos != null && notification.Tos.Any())
                message.AddTos(notification.Tos.Select(s => new EmailAddress(s)).ToList());
            if (notification.CCs != null && notification.CCs.Any())
                message.AddCcs(notification.CCs.Select(s => new EmailAddress(s)).ToList());
            message.HtmlContent = notification.Body;

            await messageCollector.AddAsync(message);
        }

        public class SmsInfoModel
        {
            public string ToPhoneNumber { get; set; }
            public string FromPhoneNumber { get; set; }
            public string Message { get; set; }
        }

        [FunctionName(nameof(SendSMS))]
        [return: TwilioSms(AccountSidSetting = "TwilioAccountSid", AuthTokenSetting = "TwilioAuthToken")]
        public CreateMessageOptions SendSMS([ActivityTrigger] SmsInfoModel notification,
            ILogger log)
        {
            log.LogInformation($"[START ACTIVITY] --> {nameof(SendSMS)}");

            var message = new CreateMessageOptions(new PhoneNumber(notification.ToPhoneNumber))
            {
                From = new PhoneNumber(notification.FromPhoneNumber),
                Body = notification.Message
            };

            return message;
        }

        public class AlertNotificationEntity : TableEntity
        {
            public AlertNotificationEntity()
            {

            }
            public AlertNotificationEntity(string alertId, CreateAlertModel alertModel)
            {
                PartitionKey = alertModel.ElfId;
                RowKey = alertId;
                AlertName = alertModel.AlertName;
                CreationTimeStamp = alertModel.CreationTimeStamp;
                Type = alertModel.Type;
                if (alertModel.Data is string)
                    Data = alertModel.Data.ToString();
                else if (alertModel.Data is JObject)
                    Data = ((JObject)alertModel.Data).ToString();
                else
                    Data = JsonConvert.SerializeObject(alertModel.Data, Formatting.Indented);
            }

            public string AlertId { get => RowKey; }
            public string ElfId { get => PartitionKey; }
            public string AlertName { get; set; }
            public DateTimeOffset? CreationTimeStamp { get; set; }
            public AlertType Type { get; set; }
            public string Data { get; set; }
        }

        [FunctionName(nameof(SaveAlertNotification))]
        public async Task SaveAlertNotification([ActivityTrigger] IDurableActivityContext context,
            [Table("notificationsArchive", Connection = "PackagesStorageAccount")] IAsyncCollector<AlertNotificationEntity> outputTable,
            ILogger log)
        {
            var (alertId, alertModel) = context.GetInput<(string, CreateAlertModel)>();
            log.LogInformation($"[START ACTIVITY] --> {nameof(SaveAlertNotification)} - AlertId={alertId}, ElfId={alertModel.ElfId}");

            var notificationEntity = new AlertNotificationEntity(alertId,alertModel);
            await outputTable.AddAsync(notificationEntity);
        }
    }
}
