using System;
using System.Threading.Tasks;
using FewBox.Core.Web.Error;

namespace FewBox.Core.Web.Filter
{
    public class ConsoleExceptionHandler : IExceptionHandler
    {
        private IExceptionProcessorService ExceptionProcessorService { get; set; }
        public ConsoleExceptionHandler(IExceptionProcessorService exceptionProcessorService)
        {
            this.ExceptionProcessorService = exceptionProcessorService;
        }
        public void Handle(string name, Exception exception)
        {
            string param = this.ExceptionProcessorService.DigInnerException(exception);
            Task.Run(() =>
            {
                ConsoleColor consoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[FewBox-Exception] {name} - {param}");
                Console.ForegroundColor = consoleColor;
            });
        }
    }
}
