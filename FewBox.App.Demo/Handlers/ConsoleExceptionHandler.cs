using System;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;

namespace FewBox.App.Demo.Handlers
{
    public class ConsoleExceptionHandler : IExceptionHandler
    {
        public ErrorResponseDto Handle(Exception exception)
        {
            Console.WriteLine(exception.Message);
            return new ErrorResponseDto(exception.Message);
        }
    }
}