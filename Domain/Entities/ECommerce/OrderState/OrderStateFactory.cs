namespace Domain.Entities.ECommerce.OrderState
{
    public static class OrderStateFactory
    {
        public static OrderState CreateState(Order order)
        {
            return order.Status switch
            {
                OrderStatus.NotPaid => new NotPaidState(order),
                OrderStatus.Paid => new PaidState(order),
                OrderStatus.Failed => new FailedState(order),
                OrderStatus.Success => new SuccessState(order),
                _ => throw new InvalidOperationException($"Unsupported order status: {order.Status}")
            };
        }
    }
}