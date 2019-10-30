namespace FewBox.Core.Web.Filter
{
    public interface ITraceHandler
    {
        void Trace(string name, object argument);
    }
}
