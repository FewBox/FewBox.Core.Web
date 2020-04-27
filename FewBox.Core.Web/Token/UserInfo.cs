﻿using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace FewBox.Core.Web.Token
{
    public struct UserInfo
    {
        private string key;
        public object Id { get; set; }
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
        public string Issuer { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }
}
