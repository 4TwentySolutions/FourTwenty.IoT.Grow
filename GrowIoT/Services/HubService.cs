using System.Threading.Tasks;
using GrowIoT.Hubs;
using GrowIoT.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GrowIoT.Services
{
    public class HubService : IHubService
    {
        private readonly IHubContext<CommandsHub> _hubContext;

        public HubService(IHubContext<CommandsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessage(string key, params object[] value)
        {
            await _hubContext.Clients.All.SendCoreAsync(key, value);
        }
    }
}
