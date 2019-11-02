using FewBox.Core.Utility.Formatter;
using FewBox.Core.Web.Log;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class TraceAsyncFilter : IAsyncActionFilter
    {
        private ILogHandler LogHandler { get; set; }

        public TraceAsyncFilter(ILogHandler logHandler)
        {
            this.LogHandler = logHandler;
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
                    string name = $"[{Environment.MachineName}] {controller}-{action}-{argument.GetType()}";
                    string prama = JsonUtility.Serialize(argument);
                    this.LogHandler.Handle(name, prama);
                }
            }
            var resultContext = await next();
        }
    }
}
