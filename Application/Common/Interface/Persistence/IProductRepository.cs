using Domain.Entities.ECommerce;

namespace Application.Common.Interface.Persistence
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsByIdsAsync(IEnumerable<string> productIds);
        // GetFirstNProductsAsync
        Task<List<Product>> GetFirstNProductsAsync(int n);
        Task<Product?> GetProductByIdAsync(Guid productId);
    }
}