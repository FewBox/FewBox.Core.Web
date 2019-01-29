namespace FewBox.Core.Web.Token
{
    public class UserProfile
    {
        public string Issuer { get; set; }
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
    }
}