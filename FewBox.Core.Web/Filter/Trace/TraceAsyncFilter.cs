using FewBox.Core.Utility.Formatter;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class TraceAsyncFilter : IAsyncActionFilter
    {
        private ILogger<TraceAsyncFilter> Logger { get; set; }

        public TraceAsyncFilter(ILogger<TraceAsyncFilter> logger)
        {
            this.Logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            int count = (((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo).GetCustomAttributes(false).Where(attribute => attribute is TraceAttribute).Count();
            if (count > 0)
            {
                string controller = context.ActionDescriptor.RouteValues["controller"];
                string action = context.ActionDescriptor.RouteValues["action"];
                var arguments = new List<string>();
                foreach (string key in context.ActionArguments.Keys)
                {
                    var argument = context.ActionArguments[key];
                    string argumentString = $"[{controller}-{action}-{argument.GetType()}] {JsonUtility.Serialize(argument)}";
                    arguments.Add(argumentString);
                }
                this.Logger.LogTrace($"[{controller}-{action}] {String.Join(',', arguments)}");
            }
            var resultContext = await next();
        }
    }
}
