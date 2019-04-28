using System;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;

namespace FewBox.App.Demo.Handlers
{
    public class ConsonleTraceLogger : ITraceLogger
    {
        public void Trace(string name, string param)
        {
            Console.WriteLine($"{name}-{param}");
        }
    }
}