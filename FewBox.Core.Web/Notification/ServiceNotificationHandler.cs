using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Error;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Notification
{
    public class ServiceNotificationHandler : INotificationHandler
    {
        private FewBoxConfig FewBoxConfig { get; set; }
        private ITryCatchService TryCatchService { get; set; }
        private IExceptionProcessorService ExceptionProcessorService { get; set; }
        private ILogger<ServiceNotificationHandler> Logger { get; set; }
        public ServiceNotificationHandler(FewBoxConfig fewBoxConfig, IExceptionProcessorService exceptionProcessorService, ILogger<ServiceNotificationHandler> logger)
        {
            this.FewBoxConfig = fewBoxConfig;
            this.ExceptionProcessorService = exceptionProcessorService;
            this.Logger = logger;
        }
        public void Handle(string name, string param)
        {
            Task.Run(() =>
            {
                try
                {
                    RestfulUtility.Post<NotificationRequestDto, NotificationResponseDto>($"{this.FewBoxConfig.Notification.Protocol}://{this.FewBoxConfig.Notification.Host}:{this.FewBoxConfig.Notification.Port}/api/notification", new Package<NotificationRequestDto>
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
                    string exceptionDetail = this.ExceptionProcessorService.DigInnerException(exception);
                    this.Logger.LogError(exceptionDetail);
                }
            });
        }
    }
}
