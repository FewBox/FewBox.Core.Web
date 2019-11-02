namespace FewBox.Core.Web.Log
{
    public interface ILogHandler
    {
        void Handle(string name, string param);
    }
}
