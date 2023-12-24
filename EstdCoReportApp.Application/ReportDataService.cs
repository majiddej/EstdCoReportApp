using EstdCoReportApp.Application.Contract;
using EstdCoReportApp.Application.Domain;

namespace EstdCoReportApp.Application
{
    public class ReportDataService : IReportDataService
    {
        public async Task<List<Report>> GetReports()
        {
            var list = new List<Report>();
            for (int i = 0; i < 50; i++)
            {
                var rnd = new Random().Next();
                list.Add(new Report { Id =rnd, Name = $"item{rnd}",Date = DateTime.Now.AddMilliseconds(rnd) });
            }
            return list;
        }
        public async Task<List<ChartData>> GetChartData()
        {
            var r = new Random();
            return new List<ChartData>()
        {
            new ChartData { Value = r.Next(1, 40), Label = "Ethereum" },
            new ChartData { Value = r.Next(1, 40), Label = "Tether" },
            new ChartData { Value = r.Next(1, 40), Label = "Binance Coin" },
            new ChartData { Value = r.Next(1, 40), Label = "Ripple" },
            new ChartData { Value = r.Next(1, 40), Label = "Cardano" },
            new ChartData { Value = r.Next(1, 40), Label = "Dogecoin" },
            new ChartData { Value = r.Next(1, 40), Label = "Solana" },
            new ChartData { Value = r.Next(1, 40), Label = "Litecoin" },
            new ChartData { Value = r.Next(1, 40), Label = "Tron" },
            new ChartData { Value = r.Next(1, 40), Label = "Shiba Inu" },
        };
        }
    }
}