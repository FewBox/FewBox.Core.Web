using System;
using System.Text;

namespace FewBox.Core.Web.Error
{
    public class ExceptionProcessorService : IExceptionProcessorService
    {
        public string DigInnerException(Exception exception)
        {
            StringBuilder exceptionDetail = new StringBuilder();
            BuildException(exceptionDetail, exception);
            return exceptionDetail.ToString();
        }

        private void BuildException(StringBuilder exceptionDetail, Exception exception)
        {
            exceptionDetail.AppendLine(exception.Message);
            exceptionDetail.AppendLine(exception.StackTrace);
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                BuildException(exceptionDetail, exception);
            }
        }
    }
}