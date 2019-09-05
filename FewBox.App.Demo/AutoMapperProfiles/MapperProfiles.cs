using AutoMapper;
using FewBox.App.Demo.Dtos;
using FewBox.App.Demo.Repositories;

namespace FewBox.App.Demo.AutoMapperProfiles
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