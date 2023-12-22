using EstdCoReportApp.Application.Contract;
using EstdCoReportApp.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace EstdCoReportApp.Server.Workers
{
    public class CryproRateDataWorker : BackgroundService
    {
        private readonly IHubContext<CryptoRateHub> _reportHub;
        private readonly IServiceProvider _serviceProvider;
        IMemoryCache _memoryCache;

        public CryproRateDataWorker(IHubContext<CryptoRateHub> cryptoRateHub, IServiceProvider serviceProvider, IMemoryCache memoryCache)
        {
            _reportHub = cryptoRateHub;
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var reportDataService = scope.ServiceProvider.GetRequiredService<ICryptoCurrencyRateDataService>();
                var reportData = await reportDataService.GetRates();
                _memoryCache.Set("cryptoRates", reportData, expirationTime);

                var methodName = "TransferCryptoRateData";

                
                await _reportHub.Clients.All.SendAsync(
                    methodName,
                    reportData,
                    stoppingToken);


                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}
