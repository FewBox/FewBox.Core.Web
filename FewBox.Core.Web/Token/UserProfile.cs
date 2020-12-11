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
        public string GzipApis { get; set; }
        public string GzipModules { get; set; }
        public IList<string> Apis
        {
            get
            {
                if (String.IsNullOrEmpty(this.GzipApis))
                {
                    return new List<string>();
                }
                else
                {
                    string gzip = GzipUtility.Unzip(this.GzipApis);
                    return JsonUtility.Deserialize<IList<string>>(gzip);
                }
            }
        }
        public IList<string> Modules
        {
            get
            {
                if (String.IsNullOrEmpty(this.GzipModules))
                {
                    return new List<string>();
                }
                else
                {
                    string gzip = GzipUtility.Unzip(this.GzipModules);
                    return JsonUtility.Deserialize<IList<string>>(gzip);
                }
            }
        }
    }
}