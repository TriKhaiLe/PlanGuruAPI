using Application.Email.Common;

using Domain.Entities.ECommerce.OrderState;
using MediatR;

namespace Application.Email.OrderStatusChanged
{
    public class OrderStatusChangedEventHandler(IEmailService emailService)
        : INotificationHandler<OrderStatusChangedEvent>
    {
        private const string Subject = "Order Status Changed";
        
        public Task Handle(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
        {
            var message = $"Your order {notification.OrderId} is now {notification.NewStatus}.";
            
            var emailRequest = new EmailRequest
            {
                Subject = Subject,
                HtmlContent = message,
                To = [new Recipient { Email = notification.Email }]
            };

            return emailService.SendMailAsync(emailRequest);
        }
    }
}

