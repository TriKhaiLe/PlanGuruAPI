namespace PlanGuruAPI.DTOs.ChatDTOs
{
    public class ChatMessageReadDTO
    {
        public Guid ChatMessageId { get; set; }
        public Guid UserSendId { get; set; }
        public string Avatar { get; set; }
        public string? Message { get; set; }
        public string? MediaLink { get; set; }
        public DateTime SendDate { get; set; }
        public string Type { get; set; }        
    }
}
