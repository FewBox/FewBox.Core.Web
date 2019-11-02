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
                Console.WriteLine($"[FewBox-Log] {name} - {param}");
                Console.ForegroundColor = consoleColor;
            });
        }
    }
}
