using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace FewBox.Core.Web.Token
{
    public struct UserInfo
    {
        public object Id { get; set; }
        // JWT
        public string Key{ get; set; }
        public string Issuer { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }
}
