using MediatR;

namespace Domain.Entities.ECommerce.OrderState
{
    public record OrderStatusChangedEvent(Guid OrderId, string Email, OrderStatus NewStatus) : INotification;
}