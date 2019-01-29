using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.App.Demo.Configs
{
    public class JWTConfig
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
    }
}