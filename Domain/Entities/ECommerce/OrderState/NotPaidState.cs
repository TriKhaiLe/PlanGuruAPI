namespace Domain.Entities.ECommerce.OrderState
{
    public class NotPaidState(Order order) : OrderState(order)
    {
        protected override void OnAccept()
        {
            Order.Status = OrderStatus.Paid;
        }

        protected override void OnReject()
        {
            Order.Product.Quantity += Order.Quantity;
            Order.Status = OrderStatus.Failed;
        }
    }
}