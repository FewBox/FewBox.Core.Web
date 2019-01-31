using System.Collections.Generic;

namespace FewBox.Core.Web.Token
{
    public class UserProfile
    {
        public string Issuer { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}