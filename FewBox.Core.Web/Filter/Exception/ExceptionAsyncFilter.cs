using FewBox.Core.Persistence.Orm;
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
            string controller = context.ActionDescriptor.RouteValues["controller"];
            string action = context.ActionDescriptor.RouteValues["action"];
            if (resultContext.Exception != null)
            {
                string name = $"{controller}-{action}";
                var errorResponseDto = this.ExceptionHandler.Handle(name, this.GetExceptionDetail(resultContext.Exception));
                resultContext.Result = new ObjectResult(errorResponseDto);
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
