using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public class Subscribe
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }//Кто подписывается
        public Guid SubId   { get; set; }// на что подписывается
    }
}
