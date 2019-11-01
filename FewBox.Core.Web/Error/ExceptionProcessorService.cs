using System;
using System.Collections.Generic;
using System.Text;
using FewBox.Core.Utility.Net;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;

namespace FewBox.Core.Web.Error
{
    public class ExceptionProcessorService : IExceptionProcessorService
    {
        private NotificationConfig NotificationConfig { get; set; }
        public ExceptionProcessorService(NotificationConfig notificationConfig)
        {
            this.NotificationConfig = notificationConfig;
        }

        public void TryCatchInNotification(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                TryCatchInConsole(() =>
                {
                    string exceptionDetail = DigInnerException(exception);
                    RestfulUtility.Post<NotificationRequestDto, NotificationResponseDto>($"{this.NotificationConfig.Protocol}://{this.NotificationConfig.Host}:{this.NotificationConfig.Port}/api/notification", new Package<NotificationRequestDto>
                    {
                        Headers = new List<Header> { },
                        Body = new NotificationRequestDto
                        {
                            Name = $"[FewBox-Remote TryCatch In Notification]",
                            Param = exceptionDetail
                        }
                    });
                });
            }
        }

        public void TryCatchInConsole(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                ConsoleColor consoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Blue;
                string exceptionDetail = DigInnerException(exception);
                Console.WriteLine($"[FewBox-{Environment.MachineName} TryCatch In Console] {exceptionDetail}");
                Console.ForegroundColor = consoleColor;
            }
        }
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