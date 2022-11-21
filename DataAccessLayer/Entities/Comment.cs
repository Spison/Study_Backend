using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid AuthorId { get; set; }
        public User Author { get; set; } = null!;        
        public string CommentText { get; set; } = null!;
        public DateTimeOffset Created { get; set; }


    }
}
