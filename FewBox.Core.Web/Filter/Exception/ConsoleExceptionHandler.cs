using FewBox.Core.Web.Dto;
using System;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class ConsoleExceptionHandler : BaseExceptionHandler
    {
        protected override void Handle(string name, string param)
        {
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
