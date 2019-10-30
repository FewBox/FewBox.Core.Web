using FewBox.Core.Persistence.Orm;
using FewBox.Core.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Text;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class ExceptionAsyncFilter : IAsyncActionFilter
    {
        private IExceptionHandler ExceptionHandler { get; set; }

        public ExceptionAsyncFilter(IExceptionHandler exceptionHandler)
        {
            this.ExceptionHandler = exceptionHandler;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            if (resultContext.Exception != null)
            {
                string controller = context.ActionDescriptor.RouteValues["controller"];
                string action = context.ActionDescriptor.RouteValues["action"];
                string name = $"{controller}-{action}";
                this.ExceptionHandler.Handle(name, resultContext.Exception);
                resultContext.Result = new ObjectResult(new ErrorResponseDto(this.GetExceptionDetail(resultContext.Exception)));
                resultContext.ExceptionHandled = true;
            }
        }

        private string GetExceptionDetail(Exception exception)
        {
            StringBuilder exceptionDetail = new StringBuilder();
            exceptionDetail.AppendLine(exception.StackTrace);
            while (exception != null)
            {
                exceptionDetail.AppendLine(exception.Message);
                exception = exception.InnerException;
            }
            return exceptionDetail.ToString();
        }
    }
}
