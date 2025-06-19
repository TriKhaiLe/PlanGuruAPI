using MediatR;

namespace Domain.Entities.ECommerce.Events
{
    public record CancelOrderEvent(
        Guid OrderId
    ) : INotification;
    
}