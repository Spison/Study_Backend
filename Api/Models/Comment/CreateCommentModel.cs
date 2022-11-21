namespace Api.Models.Comment
{
    public class CreateCommentModel
    {       
        public Guid PostId { get; set; }
        public Guid Id { get; set; }
        public string CommentText { get; set; } = null!;
        public Guid AuthorId { get; set; }
    }
    public class CreateCommentRequest
    {
        public Guid PostId { get; set; }
        public Guid? AuthorId { get; set; }
        public string CommentText { get; set; } = null!;       
    }
}
/*
         public Guid Id { get; set; }
        public string? Description { get; set; }
        public Guid AuthorId { get; set; }
 */