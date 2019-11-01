using System;
using System.Reflection;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.Core.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthyController : ControllerBase
    {
        private HealthyConfig HealthyConfig { get; set; }

        public HealthyController(HealthyConfig healthyConfig)
        {
            this.HealthyConfig = healthyConfig;
        }

        [HttpGet]
        public PayloadResponseDto<HealthyDto> Get()
        {
            return new PayloadResponseDto<HealthyDto>
            {
                Payload = new HealthyDto
                {
                    MachineName = Environment.MachineName,
                    Version = this.HealthyConfig.Version,
                    AssemblyVersion = Assembly.GetEntryAssembly().GetName().Version.ToString(),
                    AssemblyFileVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version
                }
            };
        }
    }
}