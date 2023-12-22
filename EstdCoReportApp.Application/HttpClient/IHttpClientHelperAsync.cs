using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EstdCoReportApp.Application.HttpClient
{
    public interface IHttpClientHelperAsync
    {
        Task<TResponse> GetAsync<TResponse>(string url, Dictionary<string, string> queryParam, bool hasAuthHeader = false);
        Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest body, bool hasAuthHeader = false);
        Task<string> PutAsync(string url, bool hasAuthHeader = false) => throw new NotImplementedException();
        Task<string> DeleteAsync(string url, bool hasAuthHeader = false) => throw new NotImplementedException();
        Task<string> PatchAsync(string url, bool hasAuthHeader = false) => throw new NotImplementedException();
    }
}
