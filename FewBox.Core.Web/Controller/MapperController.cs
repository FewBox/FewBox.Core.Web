using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FewBox.Core.Web.Token;
using System;

namespace FewBox.Core.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class MapperController : ControllerBase
    {
        protected IMapper Mapper { get; set; }
        protected ITokenService TokenService { get; set; }

        protected MapperController(IMapper mapper) : this(mapper, null)
        {
        }

        protected MapperController(IMapper mapper, ITokenService tokenService)
        {
            this.Mapper = mapper;
            this.TokenService = tokenService;
        }

        protected string GetUserId()
        {
            if (this.TokenService != null)
            {
                string authorization = this.HttpContext.Request.Headers["Authorization"];
                string token = String.IsNullOrEmpty(authorization) ? null : authorization.Replace("Bearer ", String.Empty, StringComparison.OrdinalIgnoreCase);
                string userId = this.TokenService.GetUserIdByToken(token);
                return userId;
            }
            else
            {
                throw new Exception("FewBox: Please init TokenService!");
            }
        }
    }
}