using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class ServiceExceptionHandler : BaseExceptionHandler
    {
        private LogConfig LogConfig { get; set; }
        private NotificationConfig NotificationConfig { get; set; }
        public ServiceExceptionHandler(LogConfig logConfig, NotificationConfig notificationConfig)
        {
            this.LogConfig = logConfig;
            this.NotificationConfig = notificationConfig;
        }
        protected override void Handle(string name, string param)
        {
            Task.Run(() =>
            {
                TryCatch(() =>
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
                TryCatch(() =>
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
