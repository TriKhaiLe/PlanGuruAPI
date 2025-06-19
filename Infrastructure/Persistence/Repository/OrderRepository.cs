using Application.Common.Interface.Persistence;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.ECommerce;

namespace Infrastructure.Persistence.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PlanGuruDBContext _context;

        public OrderRepository(PlanGuruDBContext context)
        {
            _context = context;
        }
        
        public async Task<Order> CreateAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> GetByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(order => order.Product)
                .Include(order => order.Product.Seller)
                .Include(order => order.User)
                .FirstOrDefaultAsync(order => order.Id == orderId);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(order => order.Product)
                .Include(order => order.Product.Seller)
                .Include(order => order.User)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByUserId(Guid userId)
        {
            return await _context.Orders
                .Include(order => order.Product)
                .Include(order => order.Product.Seller)
                .Include(order => order.User)
                .Where(order => order.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Order>> GetBySellerId(Guid sellerId)
        {
            return await _context.Orders
                .Include(order => order.Product)
                .Include(order => order.Product.Seller)
                .Include(order => order.User)
                .Where(order => order.Product.SellerId == sellerId)
                .ToListAsync();
        }

        public async Task DeleteAsync(Order order)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}
