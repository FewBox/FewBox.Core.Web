using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Error;
using FewBox.Core.Web.Log;
using FewBox.Core.Web.Notification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class ExceptionAsyncFilter : IAsyncActionFilter
    {
        private ILogHandler LogHandler { get; set; }
        private INotificationHandler NotificationHandler { get; set; }
        private IExceptionProcessorService ExceptionProcessorService { get; set; }

        public ExceptionAsyncFilter(ILogHandler logHandler, INotificationHandler notificationHandler, IExceptionProcessorService exceptionProcessorService)
        {
            this.LogHandler = logHandler;
            this.NotificationHandler = notificationHandler;
            this.ExceptionProcessorService = exceptionProcessorService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            if (resultContext.Exception != null)
            {
                string controller = context.ActionDescriptor.RouteValues["controller"];
                string action = context.ActionDescriptor.RouteValues["action"];
                string name = $"[{Environment.MachineName}] {controller}-{action}";
                string param = this.ExceptionProcessorService.DigInnerException(resultContext.Exception);
                this.LogHandler.HandleException(name, param);
                this.NotificationHandler.Handle(name, param);
                resultContext.Result = new ObjectResult(new ErrorResponseDto(this.ExceptionProcessorService.DigInnerException(resultContext.Exception)));
                resultContext.ExceptionHandled = true;
            }
        }


    }
}
