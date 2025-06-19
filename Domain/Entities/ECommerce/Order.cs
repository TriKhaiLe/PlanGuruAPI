using Domain.Entities.ECommerce.OrderState;

namespace Domain.Entities.ECommerce
{
    public class Order : BaseDomainEntity<Guid>
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }  
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public string ShippingAddress { get; set; }
        public OrderStatus Status { get; set; }
        private OrderState.OrderState State => OrderStateFactory.CreateState(this);

        public void ChangeToAcceptState()
        {
            State.ChangeToAcceptState();
        }

        public void ChangeToRejectState()
        {
            State.ChangeToRejectState();
        }
    }
}