using System;
using FewBox.Core.Web.Demo.Repositories;
using FBR = FewBox.Core.Web.Demo.Repositories;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;

namespace FewBox.Core.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilterController : ControllerBase
    {
        private IFewBoxRepository FewBoxRepository { get; set; }

        public FilterController(IFewBoxRepository fewBoxRepository)
        {
            this.FewBoxRepository = fewBoxRepository;
        }

        [HttpPost("transaction")]
        [Transaction]
        public void TestTransaction()
        {
            this.FewBoxRepository.Save(new FBR.FewBox { Name = "FewBox" });
        }

        [HttpPost("trace")]
        [Trace]
        public PayloadResponseDto<string> TestTrace(TraceInfo traceInfo)
        {
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
