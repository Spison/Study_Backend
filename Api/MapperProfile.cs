using AutoMapper;
using Common;
using Api.Models.Attach;
using Api.Models.Post;
using Api.Models.User;

namespace Api
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateUserModel, DataAccessLayer.Entities.User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PasswordHash, m => m.MapFrom(s => HashHelper.GetHash(s.Password)))
                .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime))
                ;
            CreateMap<DataAccessLayer.Entities.User, UserModel>();
            CreateMap<DataAccessLayer.Entities.User, UserAvatarModel>();

            CreateMap<DataAccessLayer.Entities.Avatar, AttachModel>();

            CreateMap<DataAccessLayer.Entities.PostContent, AttachModel>();
            CreateMap<DataAccessLayer.Entities.PostContent, AttachExternalModel>();

            CreateMap<CreatePostRequest, CreatePostModel>();
            CreateMap<MetadataModel, MetadataLinkModel>();
            CreateMap<MetadataLinkModel, DataAccessLayer.Entities.PostContent>();
            CreateMap<CreatePostModel, DataAccessLayer.Entities.Post>()
                .ForMember(d => d.PostContents, m => m.MapFrom(s => s.Contents))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow))

                ;
        }
    }
}