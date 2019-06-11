using AutoMapper;
using FewBox.App.Demo.Repositories;
using FewBox.Core.Web.Controller;
using FewBox.Core.Web.Dto;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace FewBox.App.Demo.Controllers
{
    [Route("api/[controller]")]
    public class AppsController : MapperController
    {
        private IAppRepository AppRepository { get; set; }

        public AppsController(IAppRepository appRepository, IMapper mapper) : base(mapper)
        {
            this.AppRepository = appRepository;
        }

        [HttpGet]
        public PayloadResponseDto<IEnumerable<FewBox.App.Demo.Repositories.App>> Get()
        {
            return new PayloadResponseDto<IEnumerable<FewBox.App.Demo.Repositories.App>>
            {
                Payload = this.AppRepository.FindAll()
            };
        }

        [HttpGet("{id}")]
        public PayloadResponseDto<FewBox.App.Demo.Repositories.App> Get(Guid id)
        {
            return new PayloadResponseDto<FewBox.App.Demo.Repositories.App>
            {
                Payload = this.AppRepository.FindOne(id)
            };
        }

        [HttpPost]
        [Transaction]
        public PayloadResponseDto<Guid> Post([FromBody]FewBox.App.Demo.Repositories.App app)
        {
            Guid appId = this.AppRepository.Save(app);
            return new PayloadResponseDto<Guid> {
                Payload = appId
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public PayloadResponseDto<int> Put(Guid id, [FromBody]FewBox.App.Demo.Repositories.App app)
        {
            int effect;
            app.Id = id;
            var updateApp = this.AppRepository.FindOne(id);
            effect = this.AppRepository.Update(app);
            return new PayloadResponseDto<int>{
                Payload = effect
            };
        }

        [HttpDelete("{id}")]
        [Transaction]
        public PayloadResponseDto<int> Delete(Guid id)
        {
            return new PayloadResponseDto<int>{
                Payload = this.AppRepository.Recycle(id)
            };
        }
    }
}
