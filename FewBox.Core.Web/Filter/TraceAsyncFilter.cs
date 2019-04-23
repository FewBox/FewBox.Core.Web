using FewBox.Core.Utility.Formatter;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class TraceAsyncFilter : IAsyncActionFilter
    {
        private ITraceLogger TraceLogger { get; set; }

        public TraceAsyncFilter(ITraceLogger traceLogger)
        {
            this.TraceLogger = traceLogger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            string controller = context.ActionDescriptor.RouteValues["controller"];
            string action = context.ActionDescriptor.RouteValues["action"];
            int count = (((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo).GetCustomAttributes(false).Where(attribute => attribute is TraceAttribute).Count();
            if (count > 0)
            {
                foreach (string key in context.ActionArguments.Keys)
                {
                    var argument = context.ActionArguments[key];
                    string name = String.Format(@"{0}-{1}-{2}", controller, action, argument.GetType());
                    string prama = JsonUtility.Serialize(argument);
                    this.TraceLogger.Trace(name, prama);
                }
            }
        }
    }
}
