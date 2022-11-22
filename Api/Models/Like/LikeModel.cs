using Api.Models.User;
namespace Api.Models.Like
{
    public class AddLikePost
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
    public class AddLikeComment
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
    }
    public class LikePostView
    {
        public Guid PostId { get; set; }
        public int count { get; set; } = 0;
        public ICollection<UserAvatarModel>?User { get; set; }
    }
    public class LikeCommentView
    {
        public Guid CommentId { get; set; }
        public int count { get; set; } = 0;
        public ICollection<UserAvatarModel>? User { get; set; } 
    }
}

/*
     public class Like
    {
        public Guid Id { get; set; }//айди сущности лайка
        public Guid PostId { get; set; }//айди поста или коммента, который лайкнули
        public int CountLikes { get; set; }=0; //счётчик лайков
        public virtual ICollection<Guid>? UserId { get; set; } // коллекция юзеров, которые лайкнули
        public virtual ICollection<Guid>? LikeId { get; set; }// коллекция айдишников лайков
    }
 */

