using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Group : BaseEntity<Guid>
    {
        public string GroupName { get; set; }
        public Guid MasterUserId { get; set; }
        public virtual User MasterUser { get; set; }
        public ICollection<GroupUser> UsersInGroup { get; set; }        
        public ICollection<Post> PostInGroup { get; set; }  
    }
}
