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
    public class ElfsRestClient : RestClientBase
    {
        public ElfsRestClient(HttpClient httpClient, string baseUrl, string apiKey) :
            base(httpClient, baseUrl, apiKey)
        {
        }

        protected override string DefaultApiEndpoint => "api/elfs";

        public async Task<IEnumerable<ElfInfoModel>> GetElfsAsync(string filterName, CancellationToken token)
        {
            string query = string.Empty;
            if (!string.IsNullOrEmpty(filterName))
            {
                query += $"name={filterName}";
            }

            Uri uri;
            if (!string.IsNullOrEmpty(query))
                uri = this.CreateAPIUri($"{query}");
            else
                uri = this.CreateAPIUri();

            var response = await this._httpClient.GetAsync(uri, token);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var elfs = JsonConvert.DeserializeObject<List<ElfInfoModel>>(content);
                return elfs;
            }
            return null;
        }
    }
}
