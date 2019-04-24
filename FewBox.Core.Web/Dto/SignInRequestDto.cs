using System;
using FewBox.Core.Web.Security;

namespace FewBox.Core.Web.Dto
{
    public class SignInRequestDto
    {
        public SignInRequestDto()
        {
            this.ExpiredTime = ExpireTimes.Token;
        }
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public TimeSpan ExpiredTime { get; set; }
    }
}