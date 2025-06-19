namespace Domain.Entities.ECommerce.OrderState
{
    public class SuccessState(Order order) : OrderState(order)
    {
        protected override void OnAccept()
        {
        }

        protected override void OnReject()
        {
        }
        
        protected override bool IsNeedToNotifyWhenAccept => false;
        protected override bool IsNeedToNotifyWhenReject => false;
    }
}