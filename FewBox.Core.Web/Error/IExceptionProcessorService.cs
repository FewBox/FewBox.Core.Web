using System;

namespace FewBox.Core.Web.Error
{
    public interface IExceptionProcessorService
    {
        void TryCatchInNotification(Action action);
        void TryCatchInConsole(Action action);
        string DigInnerException(Exception exception);
    }
}