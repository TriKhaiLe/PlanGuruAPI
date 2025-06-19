using MediatR;

namespace Domain.Entities.ECommerce.Events
{
    public record OrderStatusChangedEvent(
        Guid OrderId,
        string SellerEmail,
        string CustomerEmail,
        OrderStatus NewStatus
    ) : INotification;
    
}