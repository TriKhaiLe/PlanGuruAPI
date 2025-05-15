namespace Domain.Entities
{
    public class BaseEntity<TId>
    {
        public virtual TId Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastModifiedAt { get; set; }
    }
}
