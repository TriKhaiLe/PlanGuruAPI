namespace Domain.Entities.ECommerce.OrderState
{
    public static class OrderStateFactory
    {
        public static OrderState CreateState(Order order, OrderStatus status)
        {
            return status switch
            {
                OrderStatus.NotPaid => new NotPaidState(order),
                OrderStatus.Paid => new PaidState(order),
                OrderStatus.Failed => new FailedState(order),
                OrderStatus.Success => new SuccessState(order),
                _ => throw new InvalidOperationException($"Unsupported order status: {status}")
            };
        }
    }
}