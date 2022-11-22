using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class LikePost
    {
        public Guid Id { get; set; }//айди лайка
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }//айди поста или коммента, который лайкнули  
    }
    public class LikeComment
    {
        public Guid Id { get; set; }
        public Guid UserId  { get; set; }
        public Guid CommentId { get; set; }       
    }
}
