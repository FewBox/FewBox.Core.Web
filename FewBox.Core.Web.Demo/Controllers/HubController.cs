using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using FewBox.Core.Web.Demo.Hubs;
using FewBox.Core.Web.Demo.Dtos;

namespace FewBox.Core.Web.Controllers
{
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class HubController : ControllerBase
    {
        private IHubContext<NotificationHub> HubContext { get; set; }

        public HubController(IHubContext<NotificationHub> hubContext)
        {
            this.HubContext = hubContext;
        }

        [HttpPost]
        public void TestTransaction([FromBody] NotificationDto notificationDto)
        {
            this.HubContext.Clients.All.SendAsync("notify", notificationDto.ClientId, notificationDto.Message);
        }        
    }
}
