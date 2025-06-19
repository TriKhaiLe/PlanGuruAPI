using Application.Common.Interface.Persistence;
using Domain.Entities.ECommerce;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly PlanGuruDBContext _context;

        public ProductRepository(PlanGuruDBContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProductsByIdsAsync(IEnumerable<string> productIds)
        {
            try
            {
                var productGuids = productIds.Select(id => Guid.Parse(id)).ToList();

                var listP = await _context.Products
                    .Where(p => productGuids.Contains(p.Id))
                    .ToListAsync();

                return listP;

            }
            catch(Exception e)
            {
                return [];
            }
        }

        // GetFirstNProductsAsync
        public async Task<List<Product>> GetFirstNProductsAsync(int n)
        {
            try
            {
                var listP = await _context.Products
                    .Take(n)
                    .ToListAsync();
                return listP;
            }
            catch (Exception e)
            {
                return [];
            }
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            return await _context.Products
                .Include(product => product.Seller)
                .FirstOrDefaultAsync(order => order.Id == productId);
        }
    }
}