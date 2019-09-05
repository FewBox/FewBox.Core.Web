using AutoMapper;
using FewBox.App.Demo.Dtos;
using FewBox.App.Demo.Repositories;
using FewBox.Core.Web.Controller;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FewBox.App.Demo.Controllers
{
    [Route("api/[controller]")]
    public class FBsController : ResourcesController<FB, Guid, FBDto, FBPersistenceDto>
    {
        public FBsController(IFBRepository fBRepository, IMapper mapper) : base(fBRepository, mapper)
        {
        }
    }
}
