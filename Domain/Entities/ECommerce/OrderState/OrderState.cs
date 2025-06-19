using Domain.Entities.ECommerce.Events;

namespace Domain.Entities.ECommerce.OrderState
{
    public abstract class OrderState(Order order)
    {
        protected Order Order { get; } = order;

        protected abstract void OnAccept();
        protected abstract void OnReject();

        public void ChangeToAcceptState()
        {
            OnAccept();
            
            if (IsNeedToNotifyWhenAccept)
            {
                NotifyOrderStatusChanged();
            }   
        }
        
        public void ChangeToRejectState()
        {
            OnReject();

            if (IsNeedToNotifyWhenReject)
            {
                NotifyOrderStatusChanged();
            }
        }

        protected virtual bool IsNeedToNotifyWhenAccept => true;
        protected virtual bool IsNeedToNotifyWhenReject => true;
        
        private void NotifyOrderStatusChanged()
        {
            Order.AddDomainEvent(new CancelOrderEvent(
                Order.Id
            ));
        }
    }
}