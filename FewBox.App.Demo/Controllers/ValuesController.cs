using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FewBox.Core.Web.Security;

namespace FewBox.App.Demo.Controllers
{
    // Header: Authorization, Bearer [JWT]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IList<Value> Values { get; set; }

        public ValuesController()
        {
            this.Values = new List<Value> { 
                new Value { Id = 1, Content = "Value1" },
                new Value { Id = 2, Content = "Value2" }
            };
        }

        [HttpGet("remoterole")]
        [RemoteRoleAuthorize(Policy="RemoteRole_WithLog")]
        public IList<Value> GetByRemoteRole()
        {
            return this.Values;
        }

        // GET api/values
        [HttpGet]
        [Authorize(Roles = "Normal")]
        public IList<Value> Get()
        {
            return this.Values;
        }

        // GET api/values/5
        [Authorize(Roles = "Normal")]
        [HttpGet("{id}")]
        public Value Get(int id)
        {
            return this.Values.Where(p=>p.Id==id).SingleOrDefault();
        }

        // POST api/values
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public dynamic Post([FromBody] Value value)
        {
            return new { IsSuccessful = true };
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public dynamic Put(int id, [FromBody] Value value)
        {
            return new { IsSuccessful = true };
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public dynamic Delete(int id)
        {
            return new { IsSuccessful = true };
        }

        public class Value
        {
            public int Id { get; set; }
            public string Content { get; set; }
        }
    }
}
