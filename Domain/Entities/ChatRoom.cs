using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ChatRoom
    {
        public Guid ChatRoomId { get; set; }
        public Guid FirstUserId { get; set; }
        public Guid SecondUserId { get; set; }
        public ICollection<ChatMessage> ChatMessages { get; set; }
    }
}
