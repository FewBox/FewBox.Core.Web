using FewBox.App.Demo.Repositories;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.App.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppsController : ControllerBase
    {
        private IAppRepository AppRepository { get; set; }

        public AppsController(IAppRepository appRepository)
        {
            this.AppRepository = appRepository;
        }

        [HttpPost]
        [Transaction]
        public void Test()
        {
            this.AppRepository.Save(new FewBox.App.Demo.Repositories.App { Name = "FewBox" });
        }
    }
}
