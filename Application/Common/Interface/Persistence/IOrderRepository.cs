using Domain.Entities.ECommerce;

namespace Application.Common.Interface.Persistence
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order);
        Task<Order?> GetByIdAsync(Guid orderId);
        Task UpdateAsync(Order order);
        Task<List<Order>> GetAllAsync();
        Task<List<Order>> GetByUserId(Guid userId);
        Task<List<Order>> GetBySellerId(Guid sellerId);
        Task DeleteAsync(Order order);
    }
}
