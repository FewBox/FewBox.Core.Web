using AutoMapper;
using FewBox.Core.Web.Demo.Dtos;
using FBR = FewBox.Core.Web.Demo.Repositories;

namespace FewBox.Core.Web.Demo.AutoMapperProfiles
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<FBR.FewBox, FewBoxDto>();
            CreateMap<PersistenceFewBoxDto, FBR.FewBox>();
        }
    }
}