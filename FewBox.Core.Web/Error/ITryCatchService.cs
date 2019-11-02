using System;

namespace FewBox.Core.Web.Error
{
    public interface ITryCatchService
    {
        void TryCatch(Action action);
        void TryCatchWithoutNotification(Action action);
    }
}