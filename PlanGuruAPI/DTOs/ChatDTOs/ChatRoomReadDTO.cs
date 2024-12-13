namespace PlanGuruAPI.DTOs.ChatDTOs
{
    public class ChatRoomReadDTO
    {
        public Guid ChatRoomId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public bool IsOnline { get; set; } = false;
    }
}
