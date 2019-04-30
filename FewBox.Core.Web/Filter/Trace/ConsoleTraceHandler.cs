using FewBox.Core.Web.Dto;
using Microsoft.Extensions.Logging;
using System;

namespace FewBox.Core.Web.Filter
{
    public class ConsoleTraceHandler : ITraceHandler
    {
        public void Trace(string name, string param)
        {
            ConsoleColor consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"FewBox: {name} - {param}");
            Console.ForegroundColor = consoleColor;
        }
    }
}
