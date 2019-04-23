using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace FewBox.Core.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class MapperController : ControllerBase
    {
        protected IMapper Mapper { get; set; }

        protected MapperController(IMapper mapper)
        {
            this.Mapper = mapper;
        }
    }
}