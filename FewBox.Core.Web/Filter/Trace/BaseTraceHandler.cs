using System;
using FewBox.Core.Utility.Formatter;

namespace FewBox.Core.Web.Filter
{
    public abstract class BaseTraceHandler : ITraceHandler
    {
        protected abstract void Trace(string name, string param);
        public void Trace(string name, object argument)
        {
            string prama = JsonUtility.Serialize(argument);
            this.Trace(name, prama);
        }
    }
}
