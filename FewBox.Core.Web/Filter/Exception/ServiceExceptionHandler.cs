using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class ServiceExceptionHandler : IExceptionHandler
    {
        private LogConfig LogConfig { get; set; }
        public ServiceExceptionHandler(LogConfig logConfig)
        {
            this.LogConfig = logConfig;
        }
        public async Task<ErrorResponseDto> Handle(string name, string param)
        {
            await Task.Run(() =>
            {
                RestfulUtility.Post<LogRequestDto, LogResponseDto>($"{this.LogConfig.Protocol}://{this.LogConfig.Host}:{this.LogConfig.Port}/api/log", new Package<LogRequestDto>
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
            return new ErrorResponseDto(param);
        }
    }
}
