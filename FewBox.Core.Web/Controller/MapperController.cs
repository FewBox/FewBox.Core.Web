using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace FewBox.Core.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    abstract class MapperController : ControllerBase
    {
        private IMapper Mapper { get; set; }

        [FromHeader(Name = "Authorization")]
        public string Authorization { get; set; }

        protected MapperController(IMapper mapper)
        {
            this.Mapper = mapper;
        }
    }
}