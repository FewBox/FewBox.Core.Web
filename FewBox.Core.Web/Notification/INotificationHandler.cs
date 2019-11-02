namespace FewBox.Core.Web.Notification
{
    public interface INotificationHandler
    {
        void Handle(string name, string param);
    }
}
