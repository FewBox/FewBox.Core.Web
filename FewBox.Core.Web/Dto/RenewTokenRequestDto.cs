using System;
using FewBox.Core.Web.Security;

namespace FewBox.Core.Web.Dto
{
    public class RenewTokenRequestDto
    {
        public RenewTokenRequestDto()
        {
            this.ExpiredTime = ExpireTimes.Token;
        }
        public TimeSpan ExpiredTime { get; set; }
    }
}