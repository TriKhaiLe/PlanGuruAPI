namespace Application.Email.Common
{
    public class EmailRequest
    {
        public List<String> To { get; set; } = [];
        public List<String> Bcc { get; set; } = [];
        public string Subject { get; set; }
        public string HtmlContent { get; set; }
    }
}