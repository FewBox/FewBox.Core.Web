using AutoMapper;
using FewBox.Core.Web.Demo.Dtos;
using FewBox.Core.Web.Demo.Repositories;

namespace FewBox.Core.Web.Demo.AutoMapperProfiles
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<FB, FBDto>();
            CreateMap<FBPersistenceDto, FB>();
        }
    }
}