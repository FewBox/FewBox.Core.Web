using System;
using System.Collections.Generic;
using FewBox.Core.Utility.Compress;
using FewBox.Core.Utility.Formatter;

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
        internal string GzipApis
        {
            get
            {
                return GzipUtility.Zip(JsonUtility.Serialize<IList<string>>(this.Apis));
            }
        }
        internal string GzipModules
        {
            get
            {
                return GzipUtility.Zip(JsonUtility.Serialize<IList<string>>(this.Modules));
            }
        }
        public IList<string> Apis { get; set; }
        public IList<string> Modules { get; set; }
    }
}