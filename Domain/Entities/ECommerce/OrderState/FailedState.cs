using System.Diagnostics;

namespace Domain.Entities.ECommerce.OrderState
{
    public class FailedState(Order order) : OrderState(order)
    {
        public override void OnAccept()
        {
            UpdateState();
        }

        public override void OnReject()
        {
            UpdateState();
        }

        private void UpdateState()
        {
            Debug.WriteLine($"Order [{Order.Id}] is in FailedState. No further transitions allowed.");
            NotifyOrderStatusChanged();
        }
    }
}