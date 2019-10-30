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
        private ITraceHandler TraceHandler { get; set; }

        public TraceAsyncFilter(ITraceHandler traceHandler)
        {
            this.TraceHandler = traceHandler;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            int count = (((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo).GetCustomAttributes(false).Where(attribute => attribute is TraceAttribute).Count();
            if (count > 0)
            {
                string controller = context.ActionDescriptor.RouteValues["controller"];
                string action = context.ActionDescriptor.RouteValues["action"];
                foreach (string key in context.ActionArguments.Keys)
                {
                    var argument = context.ActionArguments[key];
                    string name = $"{controller}-{action}-{argument.GetType()}";
                    string prama = JsonUtility.Serialize(argument);
                    this.TraceHandler.Trace(name, prama);
                }
            }
            var resultContext = await next();
        }
    }
}
