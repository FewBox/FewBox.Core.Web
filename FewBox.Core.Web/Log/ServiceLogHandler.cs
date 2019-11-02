using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Error;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Log
{
    public class ServiceLogHandler : ILogHandler
    {
        private LogConfig LogConfig { get; set; }
        private ITryCatchService TryCatchService { get; set; }
        public ServiceLogHandler(LogConfig logConfig, ITryCatchService tryCatchService)
        {
            this.LogConfig = logConfig;
            this.TryCatchService = tryCatchService;
        }
        public void Handle(string name, string param)
        {
            Task.Run(() =>
            {
                this.TryCatchService.TryCatch(() =>
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
        }
    }
}
