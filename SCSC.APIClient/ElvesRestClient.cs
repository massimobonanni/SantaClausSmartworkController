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
    public class ElvesRestClient : RestClientBase
    {
        public ElvesRestClient(HttpClient httpClient, string baseUrl, string apiKey) :
            base(httpClient, baseUrl, apiKey)
        {
        }

        protected override string DefaultApiEndpoint => "api/elfs";

        public async Task<IEnumerable<ElfInfoModel>> GetElvesAsync(string filterName, CancellationToken token)
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
                var elves = JsonConvert.DeserializeObject<List<ElfInfoModel>>(content);
                return elves;
            }
            return null;
        }

        public async Task<ElfInfoModel> GetElfAsync(string elfId, CancellationToken token)
        {
            var uri = this.CreateAPIUri(null, $"api/elfs/{elfId}");

            var response = await this._httpClient.GetAsync(uri, token);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var elf = JsonConvert.DeserializeObject<ElfInfoModel>(content);
                return elf;
            }
            return null;
        }

        public async Task<bool> PackageStartedAsync(string elfId, PackageStartedModel package, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(elfId))
                throw new ArgumentException(nameof(elfId));
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            var uri = this.CreateAPIUri(null, $"api/elfs/{elfId}/packagestarted");

            string packageJson = JsonConvert.SerializeObject(package, Formatting.None);

            var postContent = new StringContent(packageJson, Encoding.UTF8, "application/json");

            var response = await this._httpClient.PostAsync(uri, postContent, cancellationToken);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> PackageEndedAsync(string elfId, PackageEndedModel package, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(elfId))
                throw new ArgumentException(nameof(elfId));
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            var uri = this.CreateAPIUri(null, $"api/elfs/{elfId}/packageended");

            string packageJson = JsonConvert.SerializeObject(package, Formatting.None);

            var postContent = new StringContent(packageJson, Encoding.UTF8, "application/json");

            var response = await this._httpClient.PostAsync(uri, postContent, cancellationToken);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateElfAsync(CreateElfModel newElf, CancellationToken cancellationToken)
        {
            if (newElf == null)
                throw new ArgumentNullException(nameof(newElf));

            if (string.IsNullOrWhiteSpace(newElf.ElfId))
                newElf.ElfId = newElf.Configuration.Name.ToLower().Replace(' ', '_');

            var uri = this.CreateAPIUri(null, $"api/elfs");

            string packageJson = JsonConvert.SerializeObject(newElf, Formatting.None);

            var postContent = new StringContent(packageJson, Encoding.UTF8, "application/json");

            var response = await this._httpClient.PostAsync(uri, postContent, cancellationToken);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateElfAsync(string elfId,UpdateElfModel updatedElf, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(elfId))
                throw new ArgumentException(nameof(elfId));
            if (updatedElf == null)
                throw new ArgumentNullException(nameof(updatedElf));

            var uri = this.CreateAPIUri(null, $"api/elfs/{elfId}");

            string packageJson = JsonConvert.SerializeObject(updatedElf, Formatting.None);

            var putContent = new StringContent(packageJson, Encoding.UTF8, "application/json");

            var response = await this._httpClient.PutAsync(uri, putContent, cancellationToken);

            return response.IsSuccessStatusCode;
        }
    }
}
