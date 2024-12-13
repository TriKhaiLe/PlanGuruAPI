using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class GroupUser
    {
        public int Id { get; set; }
        public Guid GroupId { get; set; }
        public virtual Group Group { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }      
    }
}
