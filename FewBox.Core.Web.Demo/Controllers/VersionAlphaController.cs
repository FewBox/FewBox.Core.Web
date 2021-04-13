using Microsoft.AspNetCore.Mvc;

namespace FewBox.Core.Web.Controllers
{
    [ApiVersion("2.0-alpha1")]
    [ApiController]
    [Route("api/v{v:apiVersion}/version")]
    public class VersionAlphaController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "2.0 alpha";
        }
    }
}
