using System;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class ConsoleTraceHandler : BaseTraceHandler
    {
        protected override void Trace(string name, string param)
        {
            Task.Run(() =>
            {
                ConsoleColor consoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"[FewBox-Trace] {name} - {param}");
                Console.ForegroundColor = consoleColor;
            });
        }
    }
}
