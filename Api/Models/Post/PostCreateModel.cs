using Api.Models.Attach;
using DataAccessLayer.Entities;

namespace Api.Models.Post
{
    public class CreatePostModel
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public Guid AuthorId { get; set; }
        public bool VisibleToSubscribersOnly { get; set; }
        public List<MetadataLinkModel> Contents { get; set; } = new List<MetadataLinkModel>();

    }
    public class CreatePostRequest
    {
        public string? Description { get; set; }
        public bool VisibleToSubscribersOnly { get; set; }
        public List<MetadataModel> Contents { get; set; } = new List<MetadataModel>();

    }
}