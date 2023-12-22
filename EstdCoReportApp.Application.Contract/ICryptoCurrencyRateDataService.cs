using EstdCoReportApp.Application.Domain;

namespace EstdCoReportApp.Application.Contract
{
    public interface ICryptoCurrencyRateDataService
    {
        Task<List<CryptoRate>> GetRates();
    }
}