using MediatR;

namespace Domain.Entities
{
    public class BaseDomainEntity<TId> : BaseEntity<TId>
    {
        private readonly List<INotification> _domainEvents = [];

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents.Add(eventItem);
        }

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}