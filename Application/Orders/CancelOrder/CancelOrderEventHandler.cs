using Application.Orders.Job;
using Domain.Entities.ECommerce.Events;
using Hangfire;
using MediatR;

namespace Application.Orders.CancelOrder
{
    public class CancelOrderEventHandler(IBackgroundJobClient backgroundJobClient)
        : INotificationHandler<CancelOrderEvent>
    {
        public Task Handle(CancelOrderEvent notification, CancellationToken cancellationToken)
        {
            backgroundJobClient.Schedule<CancelOrderJob>(
                job => job.HandleAsync(notification.OrderId),
                TimeSpan.FromMinutes(5)
            );

            return Task.CompletedTask;
        }
    }
}