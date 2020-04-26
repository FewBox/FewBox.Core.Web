using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace FewBox.Core.Web.Notification
{
    public class ConsoleNotificationHandler : INotificationHandler
    {
        private ILogger<ConsoleNotificationHandler> Logger { get; set; }
        public ConsoleNotificationHandler(ILogger<ConsoleNotificationHandler> logger)
        {
            this.Logger = logger;
        }
        public void Handle(string name, string param)
        {
            Task.Run(() =>
            {
                this.Logger.LogInformation($"[FewBox-Alert] {name} - {param}");
            });
        }
    }
}
