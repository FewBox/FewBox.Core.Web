using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Error;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class ExceptionAsyncFilter : IAsyncActionFilter
    {
        private IExceptionHandler ExceptionHandler { get; set; }
        private IExceptionProcessorService ExceptionProcessorService { get; set; }

        public ExceptionAsyncFilter(IExceptionHandler exceptionHandler, IExceptionProcessorService exceptionProcessorService)
        {
            this.ExceptionHandler = exceptionHandler;
            this.ExceptionProcessorService = exceptionProcessorService;
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
                resultContext.Result = new ObjectResult(new ErrorResponseDto(this.ExceptionProcessorService.DigInnerException(resultContext.Exception)));
                resultContext.ExceptionHandled = true;
            }
        }

        
    }
}
