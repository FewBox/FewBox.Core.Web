using System;
using FewBox.App.Demo.Repositories;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.App.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private IAppRepository AppRepository { get; set; }

        public FilterController(IAppRepository appRepository)
        {
            this.AppRepository = appRepository;
        }

        [HttpPost("transaction")]
        [Transaction]
        public void TestTransaction()
        {
            this.AppRepository.Save(new FewBox.App.Demo.Repositories.App { Name = "FewBox" });
        }

        [HttpPost("trace")]
        [Trace]
        public void TestTrace(TraceInfo traceInfo)
        {
        }

        [HttpPost("exception")]
        public void TestException()
        {
            throw new Exception("Hello FewBox Exception!");
        }
    }

    public class TraceInfo
    {
        public string Name { get; set; }
    }

    public class ChildInfo
    {
        public string Value { get; set; }
    }
}
