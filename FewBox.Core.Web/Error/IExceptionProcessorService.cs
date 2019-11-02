using System;

namespace FewBox.Core.Web.Error
{
    public interface IExceptionProcessorService
    {
        string DigInnerException(Exception exception);
    }
}