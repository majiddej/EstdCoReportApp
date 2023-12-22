using EstdCoReportApp.Application.Domain;
using EstdCoReportApp.Client.Pages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace EstdCoReportApp.Server.Hubs
{
    public class CryptoRateHub : Hub<List<CryptoRate>> 
    {
        private readonly IMemoryCache _memoryCache;
        IHubContext<CryptoRateHub> _reportHub;
        private readonly string methodName = "TransferCryptoRateData";

        public CryptoRateHub(IMemoryCache memoryCache, IHubContext<CryptoRateHub> cryptoRateHub) : base()
        {
            _memoryCache = memoryCache;
            _reportHub = cryptoRateHub;
        }
        public void SendDataToAllClient()
        {
            var cacheData = _memoryCache.Get<IEnumerable<CryptoRate>>("cryptoRates");

            _reportHub.Clients.All.SendAsync(
                    methodName,
                    cacheData);
        }
        public override Task OnConnectedAsync()
        {
            SendDataToAllClient();

            return base.OnConnectedAsync();
        }
        public async Task SendMessage(string message)
        {
            
        }
    }
}
