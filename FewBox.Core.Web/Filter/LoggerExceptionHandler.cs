using FewBox.Core.Web.Dto;
using Microsoft.Extensions.Logging;
using System;

namespace FewBox.Core.Web.Filter
{
    public class LoggerExceptionHandler : BaseExceptionHandler
    {
        private ILogger Logger { get; set; }

        public LoggerExceptionHandler(ILogger logger)
        {
            this.Logger = logger;
        }

        public override ErrorResponseDto Handle(Exception exception)
        {
            string exceptionDetailString = this.GetExceptionDetail(exception);
            this.Logger.LogError(exceptionDetailString);
            return new ErrorResponseDto(exceptionDetailString);
        }
    }
}
