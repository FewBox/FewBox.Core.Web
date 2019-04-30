using FewBox.Core.Persistence.Orm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
                var errorResponseDto = this.ExceptionHandler.Handle(resultContext.Exception);
                resultContext.Result = new ObjectResult(errorResponseDto);
                resultContext.ExceptionHandled = true;
            }
        }
    }
}
