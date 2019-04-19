using System;

namespace FewBox.Core.Web.Dto
{
    public class SignInResponseDto : MetaResponseDto
    {
        public bool IsValid { get; set; }
        public string Token { get; set; }
    }
}