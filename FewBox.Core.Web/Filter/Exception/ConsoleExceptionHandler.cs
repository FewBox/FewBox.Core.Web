using FewBox.Core.Web.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class ConsoleExceptionHandler : IExceptionHandler
    {
        public async Task<ErrorResponseDto> Handle(string name, string param)
        {
            string exceptionDetailString = String.Empty;
            await Task.Run(() =>
            {
                ConsoleColor consoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"FewBox: {name} - {param}");
                Console.ForegroundColor = consoleColor;
            });
            return new ErrorResponseDto(exceptionDetailString);
        }
    }
}
