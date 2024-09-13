using AutoMapper;
using MigrationWebAPPAPI.Model;
using MigrationWebAPPAPI.MongoDataDto;

namespace MigrationWebAPPAPI.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MongoDataModel, MangoDataDto>().ReverseMap();

        }
    }
}
