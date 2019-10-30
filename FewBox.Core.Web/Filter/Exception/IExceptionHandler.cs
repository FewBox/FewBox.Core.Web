using System;

namespace FewBox.Core.Web.Filter
{
    public interface IExceptionHandler
    {
        void Handle(string name, Exception exception);
    }
}
