using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ChatMessage
    {
        public Guid ChatMessageId { get; set; }
        public Guid ChatRoomId { get; set; }
        public virtual ChatRoom ChatRoom { get; set; }
        public Guid UserSendId { get; set; }
        public DateTime SendDate { get; set; }
        public string? Message { get; set; }
        public string? MediaLink { get; set; }
        public string Type { get; set; }
    }
}
