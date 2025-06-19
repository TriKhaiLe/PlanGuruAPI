using Application.Common.Interface.Persistence;
using Domain.Entities.ECommerce;

namespace Application.Orders.Job;

public class CancelOrderJob(IOrderRepository orderRepository)
{
    public async Task HandleAsync(Guid orderId)
    {
        var order = await orderRepository.GetByIdAsync(orderId);
        if (order is not { Status: OrderStatus.NotPaid })
        {
            return;
        }

        order.ChangeToRejectState();
        
        await orderRepository.UpdateAsync(order);
    }
}