using System;
using System.Collections.Generic;
using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Notification;

namespace FewBox.Core.Web.Error
{
    public class TryCatchService : ITryCatchService
    {
        private INotificationHandler NotificationHandler { get; set; }
        private IExceptionProcessorService ExceptionProcessorService { get; set; }
        public TryCatchService(INotificationHandler notificationHandler, IExceptionProcessorService exceptionProcessorService)
        {
            this.NotificationHandler = notificationHandler;
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
                    string name = $"[FewBox-Remote TryCatch In Notification]";
                    string param = this.ExceptionProcessorService.DigInnerException(exception);
                    this.NotificationHandler.Handle(name, param);
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