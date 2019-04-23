using FewBox.Core.Persistence.Orm;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class ExceptionAsyncFilter : IAsyncActionFilter
    {
        private IExceptionHandler ExceptionHandler { get; set; }
        private IOrmSession OrmSession { get; set; }

        public ExceptionAsyncFilter(IExceptionHandler exceptionHandler, IOrmSession ormSession)
        {
            this.ExceptionHandler = exceptionHandler;
            this.OrmSession = ormSession;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            if (resultContext.Exception != null)
            {
                var errorResponseDto = this.ExceptionHandler.Handle(resultContext.Exception);
                resultContext.Result = new ObjectResult(errorResponseDto);
                resultContext.ExceptionHandled = true;
                if (this.OrmSession != null)
                {
                    this.OrmSession.Close();
                }
            }
        }
    }
}
