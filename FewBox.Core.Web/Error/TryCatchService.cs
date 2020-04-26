using System;
using FewBox.Core.Web.Notification;
using Microsoft.Extensions.Logging;

namespace FewBox.Core.Web.Error
{
    public class TryCatchService : ITryCatchService
    {
        private INotificationHandler NotificationHandler { get; set; }
        private IExceptionProcessorService ExceptionProcessorService { get; set; }
        private ILogger<TryCatchService> Logger { get; set; }
        public TryCatchService(INotificationHandler notificationHandler, IExceptionProcessorService exceptionProcessorService, ILogger<TryCatchService> logger)
        {
            this.NotificationHandler = notificationHandler;
            this.ExceptionProcessorService = exceptionProcessorService;
            this.Logger = logger;
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
                string exceptionDetail = this.ExceptionProcessorService.DigInnerException(exception);
                this.Logger.LogError(exceptionDetail);
            }
        }
    }
}