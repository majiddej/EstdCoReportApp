using EstdCoReportApp.Application.Contract;
using EstdCoReportApp.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace EstdCoReportApp.Server.Workers
{
    public class WordCloudDataWorker : BackgroundService
    {
        private readonly IHubContext<WordCloudHub> _reportHub;
        private readonly IServiceProvider _serviceProvider;
        IMemoryCache _memoryCache;

        public WordCloudDataWorker(IHubContext<WordCloudHub> hub, IServiceProvider serviceProvider, IMemoryCache memoryCache)
        {
            _reportHub = hub;
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            // URL of the website you want to check
            string url = "https://example.com";


            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var reportDataService = scope.ServiceProvider.GetRequiredService<IWordCloudDataService>();
                var reportData = await reportDataService.GenerateWordCloud("https://kifpool.me/");
                _memoryCache.Set("wordCloudImage", reportData, expirationTime);

                var methodName = "TransferWordCloudData";

                await _reportHub.Clients.All.SendAsync(
                    methodName,
                    reportData,
                    stoppingToken);


                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
}
