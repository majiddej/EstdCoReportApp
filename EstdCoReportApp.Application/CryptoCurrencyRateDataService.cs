using EstdCoReportApp.Application.Contract;
using EstdCoReportApp.Application.Domain;
using EstdCoReportApp.Application.HttpClient;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EstdCoReportApp.Application
{
    public class CryptoCurrencyRateDataService : ICryptoCurrencyRateDataService
    {
        private readonly IHttpClientHelperAsync _httpClientHelperAsync;
        public CryptoCurrencyRateDataService(IHttpClientHelperAsync httpClientHelperAsync)
        {
            _httpClientHelperAsync = httpClientHelperAsync;
        }
        public async Task<List<CryptoRate>> GetRates()
        {
            //List<Rate> data = //JsonConvert.DeserializeObject<List<Rate>>(jsonData);
            var response = await _httpClientHelperAsync.GetAsync<CryptoData>("v1/exchangerate/BTC", null);

            foreach (var item in response.rates) {
                item.Id = Guid.NewGuid().ToString();
            }
            return response.rates;
        }


    }
}