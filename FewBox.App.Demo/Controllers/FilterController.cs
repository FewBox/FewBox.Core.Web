using System;
using FewBox.App.Demo.Repositories;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.App.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private IFBRepository FBRepository { get; set; }

        public FilterController(IFBRepository fbRepository)
        {
            this.FBRepository = fbRepository;
        }

        [HttpPost("transaction")]
        [Transaction]
        public void TestTransaction()
        {
            this.FBRepository.Save(new FB { Name = "FewBox" });
        }

        [HttpPost("trace")]
        [Trace]
        public PayloadResponseDto<string> TestTrace(TraceInfo traceInfo)
        {
            Console.WriteLine("TestTrace");
            return new PayloadResponseDto<string>{
                Payload = "TestTrace"
            };
        }

        [HttpPost("exception")]
        public PayloadResponseDto<string> TestException()
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
