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
    public class FBsController : ResourcesController<IFBRepository, FB, Guid, FBDto, FBPersistenceDto>
    {
        public FBsController(IFBRepository fBRepository, IMapper mapper) : base(fBRepository, mapper)
        {
        }
    }
}