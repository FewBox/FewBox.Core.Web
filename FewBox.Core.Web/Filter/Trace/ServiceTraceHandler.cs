using System.Collections.Generic;
using System.Threading.Tasks;
using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Error;

namespace FewBox.Core.Web.Filter
{
    public class ServiceTraceHandler : BaseTraceHandler, ITraceHandler
    {
        private LogConfig LogConfig { get; set; }
        private IExceptionProcessorService ExceptionProcessorService { get; set; }
        public ServiceTraceHandler(LogConfig logConfig, IExceptionProcessorService exceptionProcessorService)
        {
            this.LogConfig = logConfig;
            this.ExceptionProcessorService = exceptionProcessorService;
        }

        protected override void Trace(string name, string param)
        {
            Task.Run(() =>
            {
                this.ExceptionProcessorService.TryCatchInNotification(() =>
                {
                    RestfulUtility.Post<LogRequestDto, LogResponseDto>($"{this.LogConfig.Protocol}://{this.LogConfig.Host}:{this.LogConfig.Port}/api/logs", new Package<LogRequestDto>
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
            });
        }
    }
}
