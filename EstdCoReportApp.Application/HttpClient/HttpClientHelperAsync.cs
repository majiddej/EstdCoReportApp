using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EstdCoReportApp.Application.HttpClient
{
    public class HttpClientHelperAsync : IHttpClientHelperAsync
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientHelperAsync(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<TResponse> GetAsync<TResponse>(string url, Dictionary<string, string> queryParam, bool hasAuthHeader = false)

        {
            using var httpClient = _httpClientFactory.CreateClient("ETC");
            httpClient.DefaultRequestHeaders.Add("X-CoinAPI-Key", "C82FF27B-6119-4D53-8255-BD784EB29377");
            var fullUrl = $"{httpClient.BaseAddress}{url}";
            var destUrl = queryParam is not null ? QueryHelpers.AddQueryString(fullUrl, queryParam) : fullUrl;
            var responseStr = await httpClient.GetAsync(destUrl).Result.Content.ReadAsStringAsync();
            var objDeserializeObject = JsonConvert.DeserializeObject<TResponse>(responseStr);

            return objDeserializeObject;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest body, bool hasAuthHeader = false)
        {
            var bodySerialize = JsonConvert.SerializeObject(body);
            var requestContent = new StringContent(bodySerialize, Encoding.UTF8, "application/json");
            using var httpClient = _httpClientFactory.CreateClient("ETC");
            var response = await httpClient.PostAsync($"{httpClient.BaseAddress}{url}", requestContent);
            //var responseStr = await httpClient.GetAsync(@"https://dummy.restapiexample.com/api/v1/employees").Result.Content.ReadAsStringAsync(); FOR TEST
            response.EnsureSuccessStatusCode();
            var responseStr = await response.Content.ReadAsStringAsync();
            var objDeserializeObject = JsonConvert.DeserializeObject<TResponse>(responseStr);

            return objDeserializeObject;
        }

    }
}
