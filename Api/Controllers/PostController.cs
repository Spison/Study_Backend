using Api.Consts;
using Api.Models.Attach;
using Api.Models.Post;
using Api.Models.Comment;
using Api.Models.Like;
using Api.Services;
using Api.Exceptions;
using Common.Extentions;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Api")]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;
        public PostController(PostService postService, LinkGeneratorService links)
        {
            _postService = postService;
            links.LinkContentGenerator = x => Url.ControllerAction<AttachController>(nameof(AttachController.GetPostContent), new
            {
                postContentId = x.Id,
            });
        }

        [HttpGet]
        public async Task<List<PostModel>> GetPosts(int skip = 0, int take = 10)
            => await _postService.GetPosts(skip, take);
        [HttpGet]
        public async Task<List<PostModel>> GetPostsWithSubsPosts( Guid userId,int skip = 0, int take = 10)
            => await _postService.GetSubscriptionsPosts(skip, take, userId);
        [HttpGet]
        public async Task<PostModel> GetPostById(Guid id)
            => await _postService.GetPostById(id);

        [HttpPost]
        public async Task CreatePost(CreatePostRequest request)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            await _postService.CreatePost(request,userId);
        }
        [HttpPost]
        public async Task CreateComment(CreateCommentRequest request)
        {
            if (!request.AuthorId.HasValue)
            {
                var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
                if (userId == default)
                    throw new Exception("Not Authorize");
                request.AuthorId = userId;
            }
            await _postService.CreateComment(request);
        }
        [HttpGet]
        public async Task<List<CommentModel>> GetComments(Guid postId)
            => await _postService.GetComments(postId);

        [HttpPost]
        public async Task AddLike(LikePostModel model)
        {
            if (model.PostId == default)
                throw new PostNotFoundException();
            if (model.UserId == default)
                throw new UserNotFoundException();
            await _postService.AddLikeToPost(model.PostId, model.UserId);
        }
        [HttpPost]
        public async Task DeleteLike(Guid postId,Guid userId)
        {
            await _postService.DelUserLikeAtPost(postId, userId);
        }
        [HttpGet]
        public async Task<int> GetCountLikesAtPost(Guid postId)
            => await _postService.GetCountLikesAtPost(postId);
        [HttpGet]
        public async Task<List<LikePostModelRequest>> GetUsersWhoLikesPost(Guid postId)
            => await _postService.GetUsersWhoLikesAtPost(postId);

    }
}