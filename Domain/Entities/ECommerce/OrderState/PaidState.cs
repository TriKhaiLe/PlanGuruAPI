using System.Diagnostics;

namespace Domain.Entities.ECommerce.OrderState
{
    public class PaidState(Order order) : OrderState(order)
    {
        public override void OnAccept()
        {
            Order.Status = OrderStatus.Success;
            Debug.WriteLine($"Order [{Order.Id}] transitioned from PaidState to SuccessState.");
            
            NotifyOrderStatusChanged();
        }

        public override void OnReject()
        {
            Order.Status = OrderStatus.Failed;
            Debug.WriteLine($"Order [{Order.Id}] transitioned from PaidState to FailedState.");
            
            NotifyOrderStatusChanged();
        }
    }
}