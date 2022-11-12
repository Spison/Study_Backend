using AutoMapper;
using Common;

namespace Api
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Models.CreateUserModel, DataAccessLayer.Entities.User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PasswordHash, m => m.MapFrom(s => HashHelper.GetHash(s.Password)))
                .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime))
                ;
            CreateMap<DataAccessLayer.Entities.User, Models.UserModel>();
            CreateMap<DataAccessLayer.Entities.Avatar, Models.AttachModel>();
        }
    }
}