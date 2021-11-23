using Newtonsoft.Json;
using SCSC.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCSC.APIClient
{
    public class AlertsRestClient : RestClientBase
    {
        public AlertsRestClient(HttpClient httpClient, string baseUrl, string apiKey) :
            base(httpClient, baseUrl, apiKey)
        {
        }

        protected override string DefaultApiEndpoint => "api/alerts";

        public async Task<IEnumerable<AlertInfoModel>> GetAlertsAsync(CancellationToken token)
        {
            Uri uri = this.CreateAPIUri();

            var response = await this._httpClient.GetAsync(uri, token);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var alerts = JsonConvert.DeserializeObject<List<AlertInfoModel>>(content);
                return alerts;
            }
            return null;
        }

        public async Task<AlertInfoModel> GetAlertAsync(string alertId, CancellationToken token)
        {
            var uri = this.CreateAPIUri(null, $"api/alerts/{alertId}");

            var response = await this._httpClient.GetAsync(uri, token);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var elf = JsonConvert.DeserializeObject<AlertInfoModel>(content);
                return elf;
            }
            return null;
        }

        public async Task<bool> CreateAlertAsync(CreateAlertModel alert, CancellationToken cancellationToken)
        {
            if (alert == null)
                throw new ArgumentNullException(nameof(alert));

            Uri uri = this.CreateAPIUri();
            string alertJson = JsonConvert.SerializeObject(alert, Formatting.None);
            var postContent = new StringContent(alertJson, Encoding.UTF8, "application/json");
            var response = await this._httpClient.PostAsync(uri, postContent, cancellationToken);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CancelAlertAsync(string alertId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(alertId))
                throw new ArgumentException(nameof(alertId));

            var uri = this.CreateAPIUri(null, $"api/alerts/{alertId}/cancel");

            var response = await this._httpClient.PutAsync(uri, null, cancellationToken);

            return response.IsSuccessStatusCode;
        }

    }
}
