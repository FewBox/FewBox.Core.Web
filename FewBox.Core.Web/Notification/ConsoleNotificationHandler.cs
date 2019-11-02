using System;
using System.Threading.Tasks;
using FewBox.Core.Web.Error;

namespace FewBox.Core.Web.Notification
{
    public class ConsoleNotificationHandler : INotificationHandler
    {
        private IExceptionProcessorService ExceptionProcessorService { get; set; }
        public ConsoleNotificationHandler(IExceptionProcessorService exceptionProcessorService)
        {
            this.ExceptionProcessorService = exceptionProcessorService;
        }
        public void Handle(string name, string param)
        {
            Task.Run(() =>
            {
                ConsoleColor consoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[FewBox-Alert] {name} - {param}");
                Console.ForegroundColor = consoleColor;
            });
        }
    }
}
