using EstdCoReportApp.Application.Contract;
using EstdCoReportApp.Application.Domain;
using EstdCoReportApp.Repository.Sqlite;
using EstdCoReportApp.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson.IO;
using Newtonsoft.Json;

namespace EstdCoReportApp.Server.Workers
{
    public class MostVolatileCryptoDataWorker : BackgroundService
    {
        private readonly IHubContext<CryptoRateHub> _reportHub;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppDbContext _context;
        private readonly string methodName = "TransferMostVolatileCryptoData";

        IMemoryCache _memoryCache;

        public MostVolatileCryptoDataWorker(IHubContext<CryptoRateHub> cryptoRateHub, IServiceProvider serviceProvider, 
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
            while (!stoppingToken.IsCancellationRequested)
            {
                var allCryptoRates = _context.CryptoRates.ToList();
                // محاسبه نوسانات بر اساس تاریخ و زمان
                var fluctuations = CalculateFluctuations(allCryptoRates);

                // یافتن رمزارز با بیشترین نوسان در 1 ساعت گذشته
                CryptoRate mostFluctuatedCrypto = FindMostFluctuatedCrypto(fluctuations, _context);


                _memoryCache.Set("mostVolatileCrypto", mostFluctuatedCrypto, expirationTime);


                
                await _reportHub.Clients.All.SendAsync(
                    methodName,
                    mostFluctuatedCrypto,
                    stoppingToken);


                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
        private Dictionary<string, decimal> CalculateFluctuations(List<CryptoRate> cryptoRates)
        {
            var fluctuations = new Dictionary<string, decimal>();

            for (int i = 1; i < cryptoRates.Count; i++)
            {
                var currentRate = cryptoRates[i].rate;
                var previousRate = cryptoRates[i - 1].rate;
                var timeDifference = cryptoRates[i].time - cryptoRates[i - 1].time;

                // محاسبه نوسان درصدی
                var fluctuationPercentage = ((currentRate - previousRate) / previousRate) * 100;

                // محاسبه نوسان در یک ساعت
                if (timeDifference.TotalHours>0)
                {
                    var fluctuationInOneHour = fluctuationPercentage / (decimal)timeDifference.TotalHours;

                    fluctuations[cryptoRates[i].asset_id_quote] = fluctuationInOneHour; 
                }
            }

            return fluctuations;
        }

        private CryptoRate FindMostFluctuatedCrypto(Dictionary<string, decimal> fluctuations, AppDbContext _context)
        {
            // یافتن رمزارز با بیشترین نوسان
            var mostFluctuatedCrypto = fluctuations.OrderByDescending(kv => kv.Value).FirstOrDefault();

                // بازیابی اطلاعات رمزارز با بیشترین نوسان
                var crypto = _context.CryptoRates
                    .Where(c => c.asset_id_quote == mostFluctuatedCrypto.Key)
                    .OrderByDescending(c => c.time)
                    .FirstOrDefault();

                return crypto;
        }
    }
}
