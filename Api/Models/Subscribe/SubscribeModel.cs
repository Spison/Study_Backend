using Api.Models.User;

namespace Api.Models.Subscribe
{
    public class SubscribeModel
    {
        public Guid UserId { get; set; }//Кто подписан
        public Guid SubId { get; set; }//На кого подписан
    }
    public class Subs
    {
        public Guid UserId { get; set; } //Кто подписан 
    }
}
