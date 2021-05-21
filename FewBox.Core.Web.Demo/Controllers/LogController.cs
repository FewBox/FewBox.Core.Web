using FewBox.Core.Web.Demo.Dtos;
using FewBox.Core.Web.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FewBox.Core.Web.Demo.Controllers
{
    [ApiController]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class LogController : ControllerBase
    {
        private ILogger Logger { get; set; }

        public LogController(ILogger<LogController> logger)
        {
            this.Logger = logger;
        }

        [HttpPost]
        public MetaResponseDto Post(TraceDto trace)
        {
            this.Logger.LogError(trace.Content);
            return new MetaResponseDto();
        }
    }
}