using System.Diagnostics;

namespace Domain.Entities.ECommerce.OrderState
{
    public class SuccessState(Order order) : OrderState(order)
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
            Debug.WriteLine($"Order [{Order.Id}] is in SucessState. No further transitions allowed.");
        }
    }
}