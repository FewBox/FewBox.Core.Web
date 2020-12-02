using System.Collections.Generic;

namespace FewBox.Core.Web.Token
{
    public class UserProfile
    {
        private string key;
        public string Key
        {
            get
            {
                return this.key;
            }
            set
            {
                if (value.Length < 16)
                {
                    throw new UserInfoKeyLengthException();
                }
                this.key = value;
            }
        }
        public string Tenant { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public IList<string> Roles { get; set; }
        public IList<string> Apis { get; set; }
        public IList<string> Modules { get; set; }
    }
}