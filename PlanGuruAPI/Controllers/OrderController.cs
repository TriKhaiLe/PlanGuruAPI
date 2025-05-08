using AutoMapper;
using Domain.Entities.ECommerce;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlanGuruAPI.DTOs.OrderDTOs;

namespace PlanGuruAPI.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly PlanGuruDBContext _context;
        private readonly IMapper _mapper;

        public OrderController(PlanGuruDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var listOrder = await _context.Orders.Include(p => p.Product).ToListAsync();
            return Ok(_mapper.Map<List<OrderReadDTO>>(listOrder));
        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _context.Orders.Include(p => p.Product).FirstOrDefaultAsync(p => p.Id == orderId);   
            if(order == null)
            {
                return NotFound("Can't find this order");
            }
            return Ok(_mapper.Map<OrderReadDTO>(order));
        }
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetListOrderByUserId(Guid userId)
        {
            var checkUser = await _context.Users.FindAsync(userId); 
            if(checkUser == null)
            {
                return BadRequest("This user is not exist");
            }
            var listOrder = await _context.Orders.Include(p => p.Product).Where(p => p.UserId == userId).ToListAsync();
            return Ok(_mapper.Map<List<OrderReadDTO>>(listOrder));
        }
        [HttpGet("shops/{shopId}")]
        public async Task<IActionResult> GetListOrderByShopId(Guid shopId)
        {
            var checkShop = await _context.Users.FindAsync(shopId);
            if (checkShop == null)
            {
                return BadRequest("This shop is not exist");
            }
            var listOrder = await _context.Orders.Include(p => p.Product).Where(p => p.Product.SellerId == shopId).ToListAsync();
            return Ok(_mapper.Map<List<OrderReadDTO>>(listOrder));
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDTO order)
        {
            var checkUser = await _context.Users.FindAsync(order.UserId);
            if (checkUser == null)
            {
                return BadRequest("This user is not exist");
            }
            var checkProduct = await _context.Products.FindAsync(order.ProductId);
            if (checkProduct == null)
            {
                return BadRequest("This product is not exist");
            }
            var newOrder = _mapper.Map<Order>(order);   
            newOrder.Id = Guid.NewGuid();   
            newOrder.TotalPrice = order.Quantity * checkProduct.Price;
            newOrder.Status = OrderStatus.NotPaid;
            newOrder.CreatedAt = DateTime.Now;
            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<OrderReadDTO>(newOrder));
        }
        
        [HttpPost("confirmPayment")]
        public async Task<IActionResult> ConfirmPayment(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId); 

            if (order == null)
            {
                return BadRequest("This order does not exist");
            }
            
            order.State.OnAccept();
            await _context.SaveChangesAsync();
    
            return Ok("Confirm order successfully");
        }

        [HttpPost("markAsFailedOrder")]
        public async Task<IActionResult> FailedOrder(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId); 

            if (order == null)
            {
                return BadRequest("This order does not exist");
            }
            
            order.State.OnReject();
            await _context.SaveChangesAsync();
            return Ok("Mark this order failed successfully");
        }
        [HttpPost("markAsSuccessOrder")]
        public async Task<IActionResult> SuccessOrder(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == orderId); 

            if (order == null)
            {
                return BadRequest("This order does not exist");
            }
            
            order.State.OnAccept();
            await _context.SaveChangesAsync();
            return Ok("Mark this order success successfully");
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> UpdateOrder(Guid orderId, OrderUpdateDTO orderUpdateDto)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("This order does not exist");
            }

            var product = await _context.Products.FindAsync(orderUpdateDto.ProductId);
            if (product == null)
            {
                return BadRequest("The specified product does not exist");
            }

            _mapper.Map(orderUpdateDto, order);
            order.TotalPrice = order.Quantity * product.Price;
            order.LastModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<OrderReadDTO>(order));
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("This order does not exist");
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return Ok("Order deleted successfully");
        }

    }
}
