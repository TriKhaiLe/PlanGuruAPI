namespace Application.Email.Common
{
    public class EmailRequest
    {
        public List<Recipient> To { get; set; } = [];
        public string Subject { get; set; }
        public string HtmlContent { get; set; }
    }
}

