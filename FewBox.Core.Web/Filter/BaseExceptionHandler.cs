using System;
using System.Text;
using FewBox.Core.Web.Dto;

namespace FewBox.Core.Web.Filter
{
    public abstract class BaseExceptionHandler : IExceptionHandler
    {
        public abstract ErrorResponseDto Handle(Exception exception);

        protected string GetExceptionDetail(Exception exception)
        {
            StringBuilder exceptionDetail = new StringBuilder();
            exceptionDetail.AppendLine(exception.StackTrace);
            while (exception != null)
            {
                exceptionDetail.AppendLine(exception.Message);
                exception = exception.InnerException;
            }
            return exceptionDetail.ToString();
        }
    }
}
