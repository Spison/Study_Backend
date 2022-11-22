using Api.Models.User;
namespace Api.Models.Like
{
    public class LikePostModel
    {
        //public Guid likeId { get; set; } // по идее можно опустить, тк лайкID генерится чисто для сущности лайка
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
    }
    public class LikePostModelRequest
    {
        public Guid? UserId { get; set; }
        //public Guid? PostId { get; set; }
    }
}
