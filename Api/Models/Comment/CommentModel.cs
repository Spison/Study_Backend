using Api.Models.User;
namespace Api.Models.Comment
{
    public class CommentModel
    {
        public Guid Id { get; set; } // айди коммента
        public Guid PostId { get; set; }
        public Guid AuthorId { get; set; }
        public string CommentText { get; set; } = null!;

    }
}
