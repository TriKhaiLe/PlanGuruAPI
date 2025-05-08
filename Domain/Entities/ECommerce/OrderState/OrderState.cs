namespace Domain.Entities.ECommerce.OrderState
{
    public abstract class OrderState(Order order)
    {
        protected Order Order { get; } = order;

        public abstract void OnAccept();
        public abstract void OnReject();

        protected void NotifyOrderStatusChanged()
        {
            Order.AddDomainEvent(new OrderStatusChangedEvent(
                Order.Id,
                Order.User.Email,
                Order.Status
            ));
        }
    }
}