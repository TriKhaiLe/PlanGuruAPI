using Domain.Entities.ECommerce.Events;

namespace Domain.Entities.ECommerce.OrderState
{
    public class InitState(Order order) : OrderState(order)
    {
        protected override void OnAccept()
        {
            if (Order.Product.Quantity <= Order.Quantity)
            {
                throw new Exception("Order quantity is less than or equal to 0");
            }
            Order.Product.Quantity -= Order.Quantity;
            Order.TotalPrice = Order.Product.Price * Order.Quantity;

            Order.Status = OrderStatus.NotPaid;
            
            Order.AddDomainEvent(new OrderStatusChangedEvent(
                Order.Id,
                Order.Product.Seller.Email,
                Order.User.Email,
                Order.Status
            ));
        }

        protected override void OnReject()
        {
            Order.Product.Quantity += Order.Quantity;
            Order.Status = OrderStatus.Failed;
        }
    }
}