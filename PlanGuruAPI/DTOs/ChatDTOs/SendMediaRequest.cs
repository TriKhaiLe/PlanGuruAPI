namespace PlanGuruAPI.DTOs.ChatDTOs
{
    public class SendMediaRequest
    {
        public Guid UserSendId { get; set; }
        public Guid UserReceiveId { get; set; }
        public List<string> Images { get; set; }    
    }
}
