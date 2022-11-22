using Api.Models.User;

namespace Api.Models.Subscribe
{
    public class SubscribeModel
    {
        public Guid UserId { get; set; }
        public Guid SubId { get; set; }
    }
    public class Subs
    {
        public Guid UserId { get; set; } //Кто подписан 
    }
}
