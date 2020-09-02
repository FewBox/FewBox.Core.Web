using System;
using System.Reflection;
using FewBox.Core.Web.Config;
using FewBox.Core.Web.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.Core.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthzController : ControllerBase
    {
        private FewBoxConfig FewBoxConfig { get; set; }

        public HealthzController(FewBoxConfig fewBoxConfig)
        {
            this.FewBoxConfig = fewBoxConfig;
        }

        /// <summary>
        /// Get healthz status.
        /// </summary>
        /// <returns>Version, machinename, assembly version and assembly file version.</returns>
        [HttpGet]
        public PayloadResponseDto<HealthyDto> Get()
        {
            return new PayloadResponseDto<HealthyDto>
            {
                Payload = new HealthyDto
                {
                    Version = this.FewBoxConfig.Healthy.Version,
                    MachineName = Environment.MachineName,
                    AssemblyVersion = Assembly.GetEntryAssembly().GetName().Version.ToString(),
                    AssemblyFileVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version
                }
            };
        }
    }
}