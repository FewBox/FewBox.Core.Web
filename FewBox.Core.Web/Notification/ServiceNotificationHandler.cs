using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Error;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Notification
{
    public class ServiceNotificationHandler : INotificationHandler
    {
        private NotificationConfig NotificationConfig { get; set; }
        private ITryCatchService TryCatchService { get; set; }
        private IExceptionProcessorService ExceptionProcessorService { get; set; }
        public ServiceNotificationHandler(NotificationConfig notificationConfig, IExceptionProcessorService exceptionProcessorService)
        {
            this.NotificationConfig = notificationConfig;
            this.ExceptionProcessorService = exceptionProcessorService;
        }
        public void Handle(string name, string param)
        {
            Task.Run(() =>
            {
                try
                {
                    RestfulUtility.Post<NotificationRequestDto, NotificationResponseDto>($"{this.NotificationConfig.Protocol}://{this.NotificationConfig.Host}:{this.NotificationConfig.Port}/api/notification", new Package<NotificationRequestDto>
                    {
                        Headers = new List<Header> { },
                        Body = new NotificationRequestDto
                        {
                            Name = name,
                            Param = param
                        }
                    });
                }
                catch (Exception exception)
                {
                    ConsoleColor consoleColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Blue;
                    string exceptionDetail = this.ExceptionProcessorService.DigInnerException(exception);
                    Console.WriteLine($"[FewBox-{Environment.MachineName} Notification Exception] {exceptionDetail}");
                    Console.ForegroundColor = consoleColor;
                }
            });
        }
    }
}
