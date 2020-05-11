using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FewBox.Core.Web.Demo.Hubs
{
    [Authorize(Policy="JWTRole_Hub")]
    public class NotificationHub : Hub
    {
        public async Task Notify(string clientId, string message)
        {
            await Clients.All.SendAsync("notify", clientId, message);
        }
    }
}