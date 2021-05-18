using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FewBox.Core.Persistence.Orm;
using FewBox.Core.Web.Dto;
using System.Collections.Generic;
using System;
using System.Linq;
using FewBox.Core.Web.Filter;
using Microsoft.AspNetCore.JsonPatch;
using FewBox.Core.Web.Token;

namespace FewBox.Core.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class ResourcesController<RI, E, D, PD> : MapperController where E : Entity where RI : IRepository<E>
    {
        protected RI Repository { get; set; }
        protected ResourcesController(RI repository, ITokenService tokenService, IMapper mapper) : base(mapper, tokenService)
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
        /// Get resources count. 
        /// </summary>
        /// <returns>Resouces count.</returns>
        [HttpGet("count/owner")]
        public virtual PayloadResponseDto<int> GetOwnerCount()
        {
            return new PayloadResponseDto<int>
            {
                Payload = this.Repository.CountByCreatedBy(this.GetUserId())
            };
        }

        /// <summary>
        /// Get all resources.
        /// </summary>
        /// <returns>All resources.</returns>
        [HttpGet]
        public virtual PayloadResponseDto<IEnumerable<D>> GetAll()
        {
            return new PayloadResponseDto<IEnumerable<D>>
            {
                Payload = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAll())
            };
        }

        /// <summary>
        /// Get all resources.
        /// </summary>
        /// <returns>All resources.</returns>
        [HttpGet("owner")]
        public virtual PayloadResponseDto<IEnumerable<D>> GetOwnerAll()
        {
            return new PayloadResponseDto<IEnumerable<D>>
            {
                Payload = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAllByCreatedBy(this.GetUserId()))
            };
        }

        /// <summary>
        /// Get all resources.
        /// </summary>
        /// <returns>All resources.</returns>
        [HttpGet("sigleorder")]
        public virtual PayloadResponseDto<IEnumerable<D>> GetOrderByAll([FromQuery] IEnumerable<string> fields, OrderTypeDto orderTypeDto)
        {
            OrderType orderType = (OrderType)orderTypeDto;
            return new PayloadResponseDto<IEnumerable<D>>
            {
                Payload = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAll(fields, orderType))
            };
        }

        [HttpGet("sigleorder/owner")]
        public virtual PayloadResponseDto<IEnumerable<D>> GetOwnerOrderByAll([FromQuery] IEnumerable<string> fields, OrderTypeDto orderTypeDto)
        {
            OrderType orderType = (OrderType)orderTypeDto;
            return new PayloadResponseDto<IEnumerable<D>>
            {
                Payload = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAllByCreatedBy(this.GetUserId(), fields, orderType))
            };
        }
        /// <summary>
        /// Get all resources.
        /// </summary>
        /// <returns>All resources.</returns>
        [HttpGet("multipleorder")]
        public virtual PayloadResponseDto<IEnumerable<D>> GetOrderByAll([FromQuery] IDictionary<string, OrderTypeDto> fieldOrderDtos)
        {
            var fieldOrders = fieldOrderDtos.ToDictionary(d => d.Key, d => (OrderType)d.Value);
            return new PayloadResponseDto<IEnumerable<D>>
            {
                Payload = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAll(fieldOrders))
            };
        }

        /// <summary>
        /// Get all resources.
        /// </summary>
        /// <returns>All resources.</returns>
        [HttpGet("multipleorder/owner")]
        public virtual PayloadResponseDto<IEnumerable<D>> GetOwnerOrderByAll([FromQuery] IDictionary<string, OrderTypeDto> fieldOrderDtos)
        {
            var fieldOrders = fieldOrderDtos.ToDictionary(d => d.Key, d => (OrderType)d.Value);
            return new PayloadResponseDto<IEnumerable<D>>
            {
                Payload = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAllByCreatedBy(this.GetUserId(), fieldOrders))
            };
        }

        /// <summary>
        /// Get resources by paging.
        /// </summary>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageRange">Page range.</param>
        /// <returns>Paging resources.</returns>
        [HttpGet("paging")]
        public virtual PayloadResponseDto<PagingDto<D>> GetPagingAll([FromQuery] int pageIndex = 1, int pageRange = 5)
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
        /// Get resources by paging.
        /// </summary>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageRange">Page range.</param>
        /// <returns>Paging resources.</returns>
        [HttpGet("paging/owner")]
        public virtual PayloadResponseDto<PagingDto<D>> GetOwnerPagingAll([FromQuery] int pageIndex = 1, int pageRange = 5)
        {
            return new PayloadResponseDto<PagingDto<D>>
            {
                Payload = new PagingDto<D>
                {
                    Items = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAllByCreatedBy(this.GetUserId(), pageIndex, pageRange)),
                    PagingCount = (int)Math.Ceiling((double)this.Repository.Count() / pageRange)
                }
            };
        }

        /// <summary>
        /// Get resources by paging.
        /// </summary>
        /// <param name="fields">Fiels.</param>
        /// <param name="orderTypeDto">Order type.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageRange">Page range.</param>
        /// <returns>Paging resources.</returns>
        [HttpGet("paging/sigleorder")]
        public virtual PayloadResponseDto<PagingDto<D>> GetPagingOrderAll([FromQuery] IEnumerable<string> fields, OrderTypeDto orderTypeDto, int pageIndex = 1, int pageRange = 5)
        {
            OrderType orderType = (OrderType)orderTypeDto;
            return new PayloadResponseDto<PagingDto<D>>
            {
                Payload = new PagingDto<D>
                {
                    Items = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAll(pageIndex, pageRange, fields, orderType)),
                    PagingCount = (int)Math.Ceiling((double)this.Repository.Count() / pageRange)
                }
            };
        }

        /// <summary>
        /// Get resources by paging.
        /// </summary>
        /// <param name="fields">Fiels.</param>
        /// <param name="orderTypeDto">Order type.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageRange">Page range.</param>
        /// <returns>Paging resources.</returns>
        [HttpGet("paging/sigleorder/owner")]
        public virtual PayloadResponseDto<PagingDto<D>> GetOwnerPagingOrderAll([FromQuery] IEnumerable<string> fields, OrderTypeDto orderTypeDto, int pageIndex = 1, int pageRange = 5)
        {
            OrderType orderType = (OrderType)orderTypeDto;
            return new PayloadResponseDto<PagingDto<D>>
            {
                Payload = new PagingDto<D>
                {
                    Items = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAllByCreatedBy(this.GetUserId(), pageIndex, pageRange, fields, orderType)),
                    PagingCount = (int)Math.Ceiling((double)this.Repository.Count() / pageRange)
                }
            };
        }

        /// <summary>
        /// Get resources by paging.
        /// </summary>
        /// <param name="fieldOrderDtos">Field orders.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageRange">Page range.</param>
        /// <returns>Paging resources.</returns>
        [HttpGet("paging/multipleorder")]
        public virtual PayloadResponseDto<PagingDto<D>> GetPagingOrderAll([FromQuery] IDictionary<string, OrderTypeDto> fieldOrderDtos, int pageIndex = 1, int pageRange = 5)
        {
            var fieldOrders = fieldOrderDtos.ToDictionary(d => d.Key, d => (OrderType)d.Value);
            return new PayloadResponseDto<PagingDto<D>>
            {
                Payload = new PagingDto<D>
                {
                    Items = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAll(pageIndex, pageRange, fieldOrders)),
                    PagingCount = (int)Math.Ceiling((double)this.Repository.Count() / pageRange)
                }
            };
        }

        /// <summary>
        /// Get resources by paging.
        /// </summary>
        /// <param name="fieldOrderDtos">Field orders.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageRange">Page range.</param>
        /// <returns>Paging resources.</returns>
        [HttpGet("paging/multipleorder/owner")]
        public virtual PayloadResponseDto<PagingDto<D>> GetOwnerPagingOrderAll([FromQuery] IDictionary<string, OrderTypeDto> fieldOrderDtos, int pageIndex = 1, int pageRange = 5)
        {
            var fieldOrders = fieldOrderDtos.ToDictionary(d => d.Key, d => (OrderType)d.Value);
            return new PayloadResponseDto<PagingDto<D>>
            {
                Payload = new PagingDto<D>
                {
                    Items = this.Mapper.Map<IEnumerable<E>, IEnumerable<D>>(this.Repository.FindAllByCreatedBy(this.GetUserId(), pageIndex, pageRange, fieldOrders)),
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
        public virtual PayloadResponseDto<Guid> Post([FromBody] PD persistenceDto)
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
        public virtual PayloadResponseDto<int> Put(Guid id, [FromBody] PD persistenceDto)
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
        public virtual MetaResponseDto Patch(Guid id, [FromBody] JsonPatchDocument<E> jsonPatchDocument)
        {
            /*
            Note: application/json-patch+json !!!
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

        /// <summary>
        /// Update whole resource.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="persistenceDto">Resource.</param>
        /// <returns>Effect rows.</returns>
        [HttpPut("{id}/owner")]
        [Transaction]
        public virtual PayloadResponseDto<int> PutOwner(Guid id, [FromBody] PD persistenceDto)
        {
            int effect = -1;
            if (this.VerifyOwner(id))
            {
                E entity = this.Mapper.Map<PD, E>(persistenceDto);
                entity.Id = id;
                effect = this.Repository.Update(entity);
            }
            else
            {
                this.HttpContext.Response.StatusCode = 403;
            }
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
        [HttpPatch("{id}/owner")]
        [Transaction]
        public virtual MetaResponseDto PatchOwner(Guid id, [FromBody] JsonPatchDocument<E> jsonPatchDocument)
        {
            /*
            Note: application/json-patch+json !!!
            [
                { "op": "replace", "path": "/Name", "value": "FewBox & Landpy" },
            ]
            */
            int effect = -1;
            if (this.VerifyOwner(id))
            {
                E theEntity = this.Repository.FindOne(id);
                jsonPatchDocument.ApplyTo(theEntity);
                effect = this.Repository.Update(theEntity);
            }
            else
            {
                this.HttpContext.Response.StatusCode = 403;
            }
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
        [HttpDelete("{id}/owner")]
        [Transaction]
        public virtual PayloadResponseDto<int> DeleteOwner(Guid id)
        {
            int effect = -1;
            if (this.VerifyOwner(id))
            {
                effect = this.Repository.Recycle(id);
            }
            else
            {
                this.HttpContext.Response.StatusCode = 403;
            }
            return new PayloadResponseDto<int>
            {
                Payload = effect
            };
        }

        protected bool VerifyOwner(Guid resourceId)
        {
            return this.VerifyOwner(this.Repository, resourceId);
        }

        protected bool VerifyOwner(RI repository, Guid resourceId)
        {
            var resource = repository.FindOne(resourceId);
            if (resource != null)
            {
                return this.GetUserId() == resource.CreatedBy;
            }
            else
            {
                return false;
            }
        }
    }
}