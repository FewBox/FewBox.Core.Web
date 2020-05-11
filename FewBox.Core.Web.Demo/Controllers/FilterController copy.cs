using System;
using FewBox.Core.Web.Demo.Repositories;
using FBR = FewBox.Core.Web.Demo.Repositories;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using FewBox.Core.Web.Demo.Hubs;

namespace FewBox.Core.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HubController : ControllerBase
    {
        private IHubContext<NotificationHub> HubContext { get; set; }

        public HubController(IHubContext<NotificationHub> hubContext)
        {
            this.HubContext = hubContext;
        }

        [HttpPost]
        public void TestTransaction([FromBody] Notification notification)
        {
            this.HubContext.Clients.All.SendAsync(notification.ClientId, notification.Message);
        }
    }

    public class Notification
    {
        public string ClientId { get; set; }
        public string Message { get; set; }
    }
}
