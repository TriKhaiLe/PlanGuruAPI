using Domain.Entities.ECommerce;
using Domain.Entities.WikiService;
using Domain.Entities;

namespace PlanGuruAPI.DTOs.ProductDTOs
{
    public class ProductReadDTO
    {
        public Guid Id { get; set; }        
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; } = 0;
        public double Price { get; set; } = 0.0;
        public string Description { get; set; } = string.Empty;

        public int Sold { get; set; } = 0;

        public Guid SellerId { get; set; }
        public Guid WikiId { get; set; }

        public List<string> ProductImages { get; set; }
    }           
}
