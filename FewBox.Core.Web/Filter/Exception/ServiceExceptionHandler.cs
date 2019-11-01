using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Error;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class ServiceExceptionHandler : IExceptionHandler
    {
        private LogConfig LogConfig { get; set; }
        private NotificationConfig NotificationConfig { get; set; }
        private IExceptionProcessorService ExceptionProcessorService { get; set; }
        public ServiceExceptionHandler(LogConfig logConfig, NotificationConfig notificationConfig, IExceptionProcessorService exceptionProcessorService)
        {
            this.LogConfig = logConfig;
            this.NotificationConfig = notificationConfig;
            this.ExceptionProcessorService = exceptionProcessorService;
        }
        public void Handle(string name, Exception exception)
        {
            string param = this.ExceptionProcessorService.DigInnerException(exception);
            Task.Run(() =>
            {
                this.ExceptionProcessorService.TryCatchInNotification(() =>
                {
                    RestfulUtility.Post<LogRequestDto, LogResponseDto>($"{this.LogConfig.Protocol}://{this.LogConfig.Host}:{this.LogConfig.Port}/api/logs", new Package<LogRequestDto>
                    {
                        Headers = new List<Header> { },
                        Body = new LogRequestDto
                        {
                            Type = LogTypeDto.Exception,
                            Name = name,
                            Param = param
                        }
                    });
                });
            });
            Task.Run(() =>
            {
                this.ExceptionProcessorService.TryCatchInConsole(() =>
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
                });
            });
        }
    }
}
