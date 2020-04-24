using AutoMapper;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Demo.Dtos;
using FewBox.Core.Web.Demo.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FewBox.Core.Web.Demo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FewBoxsController : ResourcesController<IFewBoxRepository, Repositories.FewBox, Guid, FewBoxDto, PersistenceFewBoxDto>
    {
        public FewBoxsController(IFewBoxRepository fewBoxRepository, IMapper mapper) : base(fewBoxRepository, mapper)
        {
        }
    }
}