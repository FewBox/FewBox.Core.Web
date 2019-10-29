using System;
using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public class ConsoleTraceHandler : ITraceHandler
    {
        public async Task Trace(string name, string param)
        {
            await Task.Run(() =>
            {
                ConsoleColor consoleColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"FewBox: {name} - {param}");
                Console.ForegroundColor = consoleColor;
            });
        }
    }
}
