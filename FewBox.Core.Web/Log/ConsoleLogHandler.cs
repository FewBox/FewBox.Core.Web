using System;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Log
{
    public class ConsoleLogHandler : ILogHandler
    {
        public void Handle(string name, string param)
        {
            Task.Run(() =>
            {
                ConsoleColor consoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[FewBox-TraceLog] {name} - {param}");
                Console.ForegroundColor = consoleColor;
            });
        }

        public void HandleException(string name, string param)
        {
            Task.Run(() =>
            {
                ConsoleColor consoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[FewBox-ExceptionLog] {name} - {param}");
                Console.ForegroundColor = consoleColor;
            });
        }
    }
}
