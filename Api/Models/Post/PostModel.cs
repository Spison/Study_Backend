using Api.Models.Attach;
using Api.Models.User;
using Api.Models.Comment;
using Api.Models.Like;

namespace Api.Models.Post
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public UserAvatarModel Author { get; set; } = null!;
        public List<AttachExternalModel>? Contents { get; set; } = new List<AttachExternalModel>();
        public List<CommentModel>? Comments { get; set; } = new List<CommentModel>();
        public LikePostModel? Like { get; set; }
    }

}