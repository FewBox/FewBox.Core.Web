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
    public abstract class ResourcesController<RI, E, D, PD> : MapperController where E : Entity where RI : IRepository<E>
    {
        protected RI Repository { get; set; }
        protected ResourcesController(RI repository, IMapper mapper) : base(mapper)
        {
            this.Repository = repository;
        }

        /// <summary>
        /// Get resources count. 
        /// </summary>
        /// <returns>Resouces count.</returns>
        [HttpGet("count")]
        public virtual PayloadResponseDto<int> GetCount()
        {
            return new PayloadResponseDto<int>
            {
                Payload = this.Repository.Count()
            };
        }

        /// <summary>
        /// Get all resources.
        /// </summary>
        /// <returns>All resources.</returns>
        [HttpGet]
        public virtual PayloadResponseDto<IEnumerable<D>> Get()
        {
            return new PayloadResponseDto<IEnumerable<D>>
            {
                Payload = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAll())
            };
        }

        /// <summary>
        /// Get resources by paging.
        /// </summary>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageRange">Page range.</param>
        /// <returns>Paging resources.</returns>
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

        /// <summary>
        /// Get resource by id.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>Resource.</returns>
        [HttpGet("{id}")]
        public PayloadResponseDto<D> Get(Guid id)
        {
            return new PayloadResponseDto<D>
            {
                Payload = this.Mapper.Map<E, D>(this.Repository.FindOne(id))
            };
        }

        /// <summary>
        /// Save resource.
        /// </summary>
        /// <param name="persistenceDto">Resouce.</param>
        /// <returns>Id payload.</returns>
        [HttpPost]
        [Transaction]
        public virtual PayloadResponseDto<Guid> Post([FromBody]PD persistenceDto)
        {
            Guid id = this.Repository.Save(this.Mapper.Map<PD, E>(persistenceDto));
            return new PayloadResponseDto<Guid>
            {
                Payload = id
            };
        }

        /// <summary>
        /// Update whole resource.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="persistenceDto">Resource.</param>
        /// <returns>Effect rows.</returns>
        [HttpPut("{id}")]
        [Transaction]
        public virtual PayloadResponseDto<int> Put(Guid id, [FromBody]PD persistenceDto)
        {
            E entity = this.Mapper.Map<PD, E>(persistenceDto);
            entity.Id = id;
            int effect = this.Repository.Update(entity);
            return new PayloadResponseDto<int>
            {
                Payload = effect
            };
        }

        /// <summary>
        /// Update part resource.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="jsonPatchDocument">Json patch document.</param>
        /// <returns>Empty.</returns>
        [HttpPatch("{id}")]
        [Transaction]
        public virtual MetaResponseDto Patch(Guid id, [FromBody]JsonPatchDocument<E> jsonPatchDocument)
        {
            /*
            [
                { "op": "replace", "path": "/Name", "value": "FewBox & Landpy" },
            ]
            */
            E theEntity = this.Repository.FindOne(id);
            jsonPatchDocument.ApplyTo(theEntity);
            int effect = this.Repository.Update(theEntity);
            return new PayloadResponseDto<int>
            {
                Payload = effect
            };
        }

        /// <summary>
        /// Delete resource.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>Effect rows.</returns>
        [HttpDelete("{id}")]
        [Transaction]
        public virtual PayloadResponseDto<int> Delete(Guid id)
        {
            int effect = this.Repository.Recycle(id);
            return new PayloadResponseDto<int>
            {
                Payload = effect
            };
        }
    }
}