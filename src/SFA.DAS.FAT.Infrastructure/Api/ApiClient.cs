using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SFA.DAS.FAT.Domain.Configuration;
using SFA.DAS.FAT.Domain.Interfaces;

namespace SFA.DAS.FAT.Infrastructure.Api
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly FindApprenticeshipTrainingApi _config;

        public ApiClient(HttpClient httpClient, IOptions<FindApprenticeshipTrainingApi> config)
        {
            _httpClient = httpClient;
            _config = config.Value;
        }

        public async Task<TResponse> Get<TResponse>(IGetApiRequest request)
        {

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, request.GetUrl);
            AddAuthenticationHeader(requestMessage);

            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            if (response.StatusCode.Equals(HttpStatusCode.NotFound))
            {
                return default;
            }

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<TResponse>(json);
            }

            response.EnsureSuccessStatusCode();

            return default;
        }

        public async Task<TResponse> Post<TResponse, TPostData>(IPostApiRequest<TPostData> request)
        {
            var stringContent = request.Data != null ? new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json") : null;

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, request.PostUrl)
            {
                Content = stringContent
            };
            AddAuthenticationHeader(requestMessage);


            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        public async Task<TResponse> Delete<TResponse>(IDeleteApiRequest request)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Delete, request.DeleteUrl);
            AddAuthenticationHeader(requestMessage);

            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        public async Task<int> Ping()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_config.PingUrl}ping");
            AddAuthenticationHeader(requestMessage);

            var response = await _httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            return (int)response.StatusCode;
        }

        private void AddAuthenticationHeader(HttpRequestMessage httpRequestMessage)
        {
            httpRequestMessage.Headers.Add("Ocp-Apim-Subscription-Key", _config.Key);
            httpRequestMessage.Headers.Add("X-Version", "1");
        }
    }
}
