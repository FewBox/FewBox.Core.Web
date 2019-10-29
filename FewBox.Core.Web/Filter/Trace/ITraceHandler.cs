using System.Threading.Tasks;

namespace FewBox.Core.Web.Filter
{
    public interface ITraceHandler
    {
        Task Trace(string name, string param);
    }
}
