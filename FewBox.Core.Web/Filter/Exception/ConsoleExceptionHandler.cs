using FewBox.Core.Web.Dto;
using Microsoft.Extensions.Logging;
using System;

namespace FewBox.Core.Web.Filter
{
    public class ConsoleExceptionHandler : BaseExceptionHandler
    {
        public override ErrorResponseDto Handle(Exception exception)
        {
            string exceptionDetailString = this.GetExceptionDetail(exception);
            ConsoleColor consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"FewBox: {exceptionDetailString}");
            Console.ForegroundColor = consoleColor;
            return new ErrorResponseDto(exceptionDetailString);
        }
    }
}
