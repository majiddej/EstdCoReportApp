using EstdCoReportApp.Application.Domain;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace EstdCoReportApp.Server.Hubs
{
    public class ChartHub : Hub 
    {
        IHubContext<ChartHub> _reportHub;
        private readonly string methodName = "TransferChartData";
        private readonly IMemoryCache _memoryCache;
        public ChartHub(IMemoryCache memoryCache, IHubContext<ChartHub> reportHub) : base()
        {
            _memoryCache = memoryCache;
            _reportHub = reportHub;
        }
        public void SendDataToAllClient()
        {
            var cacheData = _memoryCache.Get<IEnumerable<CryptoRate>>("cryptoRates");

            _reportHub.Clients.All.SendAsync(
                    methodName,
                    cacheData);
        }
        public void SendDataToClient(string connectionId)
        {
            var cacheData = _memoryCache.Get<IEnumerable<CryptoRate>>("cryptoRates");

            _reportHub.Clients.Client(connectionId).SendAsync(
                    methodName,
                    cacheData);
        }
        public override Task OnConnectedAsync()
        {
            //SendDataToClient(Context.ConnectionId);
            return base.OnConnectedAsync();
        }
        public async Task SendMessage(string message)
        {

        }
    }
}
