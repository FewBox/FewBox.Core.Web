using System;

namespace FewBox.Core.Web.Token
{
    public class UserInfoKeyLengthException : Exception
    {
        public UserInfoKeyLengthException() : base("Please set a UserInfo key which is greater than 16!")
        {
        }
    }
}
