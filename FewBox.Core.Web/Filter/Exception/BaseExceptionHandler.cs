using System;
using System.Text;

namespace FewBox.Core.Web.Filter
{
    public abstract class BaseExceptionHandler : BaseHandler, IExceptionHandler
    {
        protected abstract void Handle(string name, string param);
        public void Handle(string name, Exception exception)
        {
            this.Handle(name, this.GetExceptionDetail(exception));
        }
    }
}
