using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mercury.Models;
using Newtonsoft.Json;

namespace Mercury.Client
{
    public class MercuryClient : IMercuryClient
    {
        private const string JsonMediaType = "application/json";

        private readonly HttpClient httpClient;
        private readonly MercuryClientSettings settings;

        public MercuryClient(HttpClient httpClient, MercuryClientSettings settings)
        {
            this.httpClient = httpClient;
            this.settings = settings;
        }

        public async Task<MercuryResult> SendAsync<TPayload>(MercuryRequest<TPayload> request, CancellationToken token = default)
            where TPayload : class
        {
            var content = GetHttpContent(request);

            var response = await httpClient
                .PostAsync($"api/v{settings.Version}/send", content)
                .ConfigureAwait(false);

            return await DeserializeAsync(response).ConfigureAwait(false);
        }

        public async Task<MercuryResult> QueueAsync<TPayload>(MercuryRequest<TPayload> request, CancellationToken token = default)
            where TPayload : class
        {
            var content = GetHttpContent(request);

            var response = await httpClient
                .PostAsync($"api/v{settings.Version}/queue", content)
                .ConfigureAwait(false);

            return await DeserializeAsync(response).ConfigureAwait(false);
        }

        private static HttpContent GetHttpContent<TPayload>(MercuryRequest<TPayload> request)
            where TPayload : class
        {
            var json = JsonConvert.SerializeObject(request);
            return new StringContent(json, Encoding.UTF8, JsonMediaType);
        }

        private static async Task<MercuryResult> DeserializeAsync(HttpResponseMessage response)
        {
            using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            using (var streamReader = new StreamReader(contentStream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer();

                return serializer.Deserialize<MercuryResult>(jsonReader);
            }
        }
    }
}