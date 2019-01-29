using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.App.Demo.Dtos
{
    public class SignInRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}