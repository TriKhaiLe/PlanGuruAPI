namespace Domain.Entities.ECommerce.OrderState
{
    public class PaidState(Order order) : OrderState(order)
    {
        protected override void OnAccept()
        {
            Order.Product.Sold += Order.Quantity;
            Order.Status = OrderStatus.Success;
        }

        protected override void OnReject()
        {
            Order.Product.Quantity += Order.Quantity;
            Order.Status = OrderStatus.Failed;
        }
    }
}