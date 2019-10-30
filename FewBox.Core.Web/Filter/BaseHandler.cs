using System;
using System.Text;

namespace FewBox.Core.Web.Filter
{
    public abstract class BaseHandler
    {
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

        protected void TryCatch(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                ConsoleColor consoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Blue;
                string exceptionDetail = this.GetExceptionDetail(exception);
                Console.WriteLine($"[FewBox-Remote Exception] {exceptionDetail}");
                Console.ForegroundColor = consoleColor;
            }
        }
    }
}
