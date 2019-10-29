using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;

namespace FewBox.Core.Web.Filter
{
    public class ServiceTraceHandler : ITraceHandler
    {
        private LogConfig LogConfig { get; set; }
        public ServiceTraceHandler(LogConfig logConfig)
        {
            this.LogConfig = logConfig;
        }

        public async Task Trace(string name, string param)
        {
            await Task.Run(() =>
            {
                RestfulUtility.Post<LogRequestDto, LogResponseDto>($"{this.LogConfig.Protocol}://{this.LogConfig.Host}:{this.LogConfig.Port}/api/log", new Package<LogRequestDto>
                {
                    Headers = new List<Header> { },
                    Body = new LogRequestDto
                    {
                        Type = LogTypeDto.Audit,
                        Name = name,
                        Param = param
                    }
                });
            });
        }
    }
}
