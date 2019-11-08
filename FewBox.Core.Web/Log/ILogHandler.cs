namespace FewBox.Core.Web.Log
{
    public interface ILogHandler
    {
        void Handle(string name, string param);
        void HandleException(string name, string param);
    }
}
