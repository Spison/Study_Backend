using Api.Mapper.MapperActions;
using Api.Models.Attach;
using Api.Models.Post;
using Api.Models.User;
using Api.Models.Comment;
using Api.Models.Like;
using Api.Models.Subscribe;
using AutoMapper;
using Common;
using DataAccessLayer.Entities;

namespace Api.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateUserModel, User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PasswordHash, m => m.MapFrom(s => HashHelper.GetHash(s.Password)))
                .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime))
                ;
            CreateMap<User, UserModel>();
            CreateMap<User, UserAvatarModel>()
                .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate))
                .ForMember(d => d.PostsCount, m => m.MapFrom(s => s.Posts!.Count))
                .AfterMap<UserAvatarMapperAction>();

            CreateMap<Avatar, AttachModel>();
            CreateMap<Post, PostModel>()
                .ForMember(d => d.Contents, m => m.MapFrom(d => d.PostContents))
                .ForMember(d=>d.VisibleToSubscribersOnly, m => m.MapFrom(d=>d.VisibleToSubscribersOnly))
                ;
            CreateMap<PostModel, Post>()
                .ForMember(d => d.VisibleToSubscribersOnly, m => m.MapFrom(d => d.VisibleToSubscribersOnly));
            CreateMap<PostContent, AttachModel>();
            CreateMap<PostContent, AttachExternalModel>().AfterMap<PostContentMapperAction>();

            CreateMap<CreatePostRequest, CreatePostModel>();
            CreateMap<MetadataModel, MetadataLinkModel>();
            CreateMap<MetadataLinkModel, PostContent>();
            CreateMap<CreatePostModel, Post>()
                .ForMember(d => d.PostContents, m => m.MapFrom(s => s.Contents))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTime.UtcNow));

            
            CreateMap<CreateCommentRequest,CreateCommentModel>();
            CreateMap<Comment, CommentModel>();
            CreateMap<CreateCommentModel,Comment>()
                .ForMember(d=>d.AuthorId,m=>m.MapFrom(s=>s.AuthorId))
                .ForMember(d=>d.Created,m => m.MapFrom(s=>DateTime.UtcNow))
                ;

            CreateMap<LikePostModel, LikePost>()
                .ForMember(d => d.UserId, m => m.MapFrom(s => s.UserId))
                .ForMember(d => d.PostId, m => m.MapFrom(s => s.PostId))
                ;
            //Потом добавить маппинг для коммента
            //CreateMap<LikeCommentModel, LikePost>()
            //    .ForMember(d => d.UserId, m => m.MapFrom(s => s.UserId))
            //    .ForMember(d => d.PostId, m => m.MapFrom(s => s.PostId))
            //    ;
            CreateMap<SubscribeModel, Subscribe>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                ;
            CreateMap<Subscribe, SubscribeModel>()
                .ForMember(d => d.SubId, m => m.MapFrom(m => m.SubId))
                .ForMember(d => d.UserId, m => m.MapFrom(m => m.UserId))
                ;
            CreateMap<SubscribeModel, Subs>()
                .ForMember(d => d.UserId, m => m.MapFrom(s => s.SubId));


        }


    }
}