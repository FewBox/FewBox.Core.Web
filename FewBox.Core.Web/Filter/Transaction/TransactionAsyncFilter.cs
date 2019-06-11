using FewBox.Core.Persistence.Orm;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class TransactionAsyncFilter : IAsyncActionFilter
    {
        private IOrmSession OrmSession { get; set; }

        public TransactionAsyncFilter(IOrmSession ormSession)
        {
            this.OrmSession = ormSession;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            this.OrmSession.UnitOfWork.Start();
            var resultContext = await next();
            int count = (((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo).GetCustomAttributes(false).Where(attribute => attribute is TransactionAttribute).Count();
            if (count > 0)
            {
                if (resultContext.Exception == null)
                {
                    this.OrmSession.UnitOfWork.Commit();
                }
                else
                {
                    this.OrmSession.UnitOfWork.Rollback();
                }
            }
            this.OrmSession.UnitOfWork.Stop();
        }
    }

}
