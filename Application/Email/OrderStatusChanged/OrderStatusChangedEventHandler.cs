using Application.Email.Common;
using Domain.Entities.ECommerce.Events;
using MediatR;

namespace Application.Email.OrderStatusChanged
{
    public class OrderStatusChangedEventHandler(IEmailService emailService)
        : INotificationHandler<OrderStatusChangedEvent>
    {
        private const string Subject = "Order Status Changed";
        
        public Task Handle(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
        {
            var message = $"Order {notification.OrderId} is now at {notification.NewStatus}.";
            
            var emailRequest = new EmailRequest
            {
                Subject = Subject,
                HtmlContent = message,
                To = [notification.CustomerEmail],
                Bcc = [notification.SellerEmail]
            };

            return emailService.SendMailAsync(emailRequest);
        }
    }
}