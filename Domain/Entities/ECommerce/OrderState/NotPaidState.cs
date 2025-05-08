using System.Diagnostics;

namespace Domain.Entities.ECommerce.OrderState
{
    public class NotPaidState(Order order) : OrderState(order)
    {
        public override void OnAccept()
        {
            Order.Status = OrderStatus.Paid;
            Debug.WriteLine($"Order [{Order.Id}] transitioned from NotPaidState to SuccessState.");
            
            NotifyOrderStatusChanged();
        }

        public override void OnReject()
        {
            Order.Status = OrderStatus.Failed;
            Debug.WriteLine($"Order [{Order.Id}] transitioned from NotPaidState to FailedState.");
            
            NotifyOrderStatusChanged();
        }
    }
}