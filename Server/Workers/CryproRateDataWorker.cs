using EstdCoReportApp.Application.Contract;
using EstdCoReportApp.Repository.Sqlite;
using EstdCoReportApp.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson.IO;
using Newtonsoft.Json;

namespace EstdCoReportApp.Server.Workers
{
    public class CryproRateDataWorker : BackgroundService
    {
        private readonly IHubContext<CryptoRateHub> _reportHub;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppDbContext _context;

        IMemoryCache _memoryCache;

        public CryproRateDataWorker(IHubContext<CryptoRateHub> cryptoRateHub, IServiceProvider serviceProvider,
            IMemoryCache memoryCache)
        {
            _reportHub = cryptoRateHub;
            _serviceProvider = serviceProvider;
            _memoryCache = memoryCache;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var expirationTime = DateTimeOffset.Now.AddMinutes(10.0);

            using var scope = _serviceProvider.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var list = _context.CryptoRates.ToList();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var reportDataService = scope.ServiceProvider.GetRequiredService<ICryptoCurrencyRateDataService>();
                    var reportData = await reportDataService.GetRates();
                    try
                    {
                        //string output = Newtonsoft.Json.JsonConvert.SerializeObject(reportData);
                        //ذخیره در جدول برای محاسبه بیشترین نوسان
                        _context.CryptoRates.AddRange(reportData);
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                    }

                    _memoryCache.Set("cryptoRates", reportData, expirationTime);

                    var methodName = "TransferCryptoRateData";


                    await _reportHub.Clients.All.SendAsync(
                        methodName,
                        reportData,
                        stoppingToken);


                }
                catch (Exception ex)
                {

                }
                await Task.Delay(TimeSpan.FromSeconds(300), stoppingToken);
            }
        }
    }
}
