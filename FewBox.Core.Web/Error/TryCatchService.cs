using System;
using System.Collections.Generic;
using System.Text;
using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;

namespace FewBox.Core.Web.Error
{
    public class TryCatchService : ITryCatchService
    {
        private NotificationConfig NotificationConfig { get; set; }
        private IExceptionProcessorService ExceptionProcessorService { get; set; }
        public TryCatchService(NotificationConfig notificationConfig, IExceptionProcessorService exceptionProcessorService)
        {
            this.NotificationConfig = notificationConfig;
            this.ExceptionProcessorService = exceptionProcessorService;
        }

        public void TryCatch(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                TryCatchWithoutNotification(() =>
                {
                    string exceptionDetail = this.ExceptionProcessorService.DigInnerException(exception);
                    RestfulUtility.Post<AlertRequestDto, AlertResponseDto>($"{this.NotificationConfig.Protocol}://{this.NotificationConfig.Host}:{this.NotificationConfig.Port}/api/notification", new Package<AlertRequestDto>
                    {
                        Headers = new List<Header> { },
                        Body = new AlertRequestDto
                        {
                            Name = $"[FewBox-Remote TryCatch In Notification]",
                            Param = exceptionDetail
                        }
                    });
                });
            }
        }

        public void TryCatchWithoutNotification(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                ConsoleColor consoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Blue;
                string exceptionDetail = this.ExceptionProcessorService.DigInnerException(exception);
                Console.WriteLine($"[FewBox-{Environment.MachineName} TryCatch In Console] {exceptionDetail}");
                Console.ForegroundColor = consoleColor;
            }
        }
    }
}