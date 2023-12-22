using EstdCoReportApp.Application.Domain;
using EstdCoReportApp.Client.Pages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace EstdCoReportApp.Server.Hubs
{
    public class WordCloudHub : Hub<string> 
    {
        private readonly IMemoryCache _memoryCache;
        IHubContext<WordCloudHub> _reportHub;
        private readonly string methodName = "TransferWordCloudData";

        public WordCloudHub(IMemoryCache memoryCache, IHubContext<WordCloudHub> hub) : base()
        {
            _memoryCache = memoryCache;
            _reportHub = hub;
        }
        public void SendDataToAllClient()
        {
            var cacheData = _memoryCache.Get<string>("wordCloudImage");

            _reportHub.Clients.All.SendAsync(
                    methodName,
                    cacheData);
        }
        public override Task OnConnectedAsync()
        {
            //TODO Get ConnectionId And Send Last Data Just To The New Connected Id
            SendDataToAllClient();

            return base.OnConnectedAsync();
        }
        public async Task SendMessage(string message)
        {
            
        }
    }
}
