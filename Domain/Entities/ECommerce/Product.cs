using Domain.Entities.WikiService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.ECommerce
{
    public class Product : BaseEntity<Guid>
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; } = 0;
        public double Price { get; set; } = 0.0;
        public string Description { get; set; } = string.Empty;

        public Guid SellerId { get; set; }
        public virtual User Seller { get; set; }
        public Guid? WikiId { get; set; }
        public virtual Wiki? Wiki { get; set; }
        public ICollection<ProductImages> ProductImages { get; set; }
        
        public int Sold { get; set; } = 0;
    }
}
