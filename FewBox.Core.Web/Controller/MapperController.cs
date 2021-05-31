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

        protected UserProfile GetUserProfile()
        {
            UserProfile userProfile = null;
            if (this.TokenService != null)
            {
                string authorization = this.HttpContext.Request.Headers["Authorization"];
                string token = String.IsNullOrEmpty(authorization) ? null : authorization.Replace("Bearer ", String.Empty, StringComparison.OrdinalIgnoreCase);
                if (token != null)
                {
                    userProfile = this.TokenService.GetUserProfileByToken(token);
                }
            }
            else
            {
                throw new Exception("FewBox: Please init TokenService!");
            }
            return userProfile;
        }

        protected Guid GetUserId()
        {
            Guid userId;
            var userProfile = this.GetUserProfile();
            if (userProfile == null)
            {
                userId = Guid.Empty;
            }
            else
            {
                Guid.TryParse(userProfile.Id, out userId);
            }
            return userId;
        }

        protected string GetUserTenant()
        {
            string userTenant;
            var userProfile = this.GetUserProfile();
            if (userProfile == null)
            {
                userTenant = String.Empty;
            }
            else
            {
                userTenant = userProfile.Tenant;
            }
            return userTenant;
        }
    }
}