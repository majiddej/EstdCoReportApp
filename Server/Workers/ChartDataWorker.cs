using EstdCoReportApp.Application.Contract;
using EstdCoReportApp.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace EstdCoReportApp.Server.Workers
{
    public class ChartDataWorker : BackgroundService
    {
        private readonly IHubContext<ChartHub> _reportHub;
        private readonly IServiceProvider _serviceProvider;


        public ChartDataWorker(IHubContext<ChartHub> chartHub, IServiceProvider serviceProvider)
        {
            _reportHub = chartHub;
            _serviceProvider = serviceProvider;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var reportDataService = scope.ServiceProvider.GetRequiredService<IReportDataService>();


                var reportData = await reportDataService.GetChartData();
                var methodName = "TransferChartData";


                await _reportHub.Clients.All.SendAsync(
                    methodName,
                    reportData,
                    stoppingToken);


                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
