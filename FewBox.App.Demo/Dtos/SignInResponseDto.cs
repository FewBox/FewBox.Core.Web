using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.App.Demo.Dtos
{
    public class SignInResponseDto
    {
        public bool IsValid { get; set; }
        public string Token { get; set; }
    }
}