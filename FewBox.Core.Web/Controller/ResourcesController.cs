using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FewBox.Core.Persistence.Orm;
using FewBox.Core.Web.Dto;
using System.Collections.Generic;
using System;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.JsonPatch;

namespace FewBox.Core.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ResourcesController<RI, E, I, D, PD> : MapperController where E : Entity<I> where RI : IBaseRepository<E, I>
    {
        protected RI Repository { get; set; }
        protected ResourcesController(RI repository, IMapper mapper) : base(mapper)
        {
            this.Repository = repository;
        }

        [HttpGet("count")]
        public virtual PayloadResponseDto<int> GetCount()
        {
            return new PayloadResponseDto<int>
            {
                Payload = this.Repository.Count()
            };
        }

        [HttpGet]
        public virtual PayloadResponseDto<IEnumerable<D>> Get()
        {
            return new PayloadResponseDto<IEnumerable<D>>
            {
                Payload = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAll())
            };
        }

        [HttpGet("paging")]
        public virtual PayloadResponseDto<PagingDto<D>> Get([FromQuery] int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<D>>
            {
                Payload = new PagingDto<D>
                {
                    Items = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAll(pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.Repository.Count() / pageRange)
                }
            };
        }

        [HttpGet("{id}")]
        public PayloadResponseDto<D> Get(I id)
        {
            return new PayloadResponseDto<D>
            {
                Payload = this.Mapper.Map<E, D>(this.Repository.FindOne(id))
            };
        }

        [HttpPost]
        [Transaction]
        public virtual PayloadResponseDto<I> Post([FromBody]PD persistenceDto)
        {
            I id = this.Repository.Save(this.Mapper.Map<PD, E>(persistenceDto));
            return new PayloadResponseDto<I>
            {
                Payload = id
            };
        }

        [HttpPut("{id}")]
        [Transaction]
        public virtual PayloadResponseDto<int> Put(I id, [FromBody]PD persistenceDto)
        {
            E entity = this.Mapper.Map<PD, E>(persistenceDto);
            entity.Id = id;
            int effect = this.Repository.Update(entity);
            return new PayloadResponseDto<int>
            {
                Payload = effect
            };
        }

        [HttpPatch("{id}")]
        [Transaction]
        public virtual MetaResponseDto Patch(I id, [FromBody]JsonPatchDocument<E> jsonPatchEntity)
        {
            E theEntity = this.Repository.FindOne(id);
            jsonPatchEntity.ApplyTo(theEntity);
            int effect = this.Repository.Update(theEntity);
            return new PayloadResponseDto<int>
            {
                Payload = effect
            };
        }

        [HttpDelete("{id}")]
        [Transaction]
        public virtual PayloadResponseDto<int> Delete(I id)
        {
            int effect = this.Repository.Recycle(id);
            return new PayloadResponseDto<int>
            {
                Payload = effect
            };
        }
    }
}