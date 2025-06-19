using Application.Email.Common;

namespace Application.Email.OrderStatusChanged
{
    public interface IEmailService
    {
        Task SendMailAsync(EmailRequest request);
    }
}

