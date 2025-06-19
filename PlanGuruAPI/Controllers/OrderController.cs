using Application.Common.Interface.Persistence;
using AutoMapper;
using Domain.Entities.ECommerce;
using Microsoft.AspNetCore.Mvc;
using PlanGuruAPI.DTOs.OrderDTOs;

namespace PlanGuruAPI.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        
        private readonly IMapper _mapper;

        public OrderController(
            IOrderRepository orderRepository, 
            IUserRepository userRepository,
            IProductRepository productRepository,
            IMapper mapper
            )
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var listOrder = await _orderRepository.GetAllAsync(); 
            return Ok(_mapper.Map<List<OrderReadDTO>>(listOrder));
        }
        
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if(order == null)
            {
                return NotFound("Can't find this order");
            }
            return Ok(_mapper.Map<OrderReadDTO>(order));
        }
        
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetListOrderByUserId(Guid userId)
        {
            var checkUser = await _userRepository.GetByIdAsync(userId);
            if(checkUser == null)
            {
                return BadRequest("This user is not exist");
            }
            
            var listOrder = await _orderRepository.GetByUserId(userId);
            return Ok(_mapper.Map<List<OrderReadDTO>>(listOrder));
        }
        
        [HttpGet("shops/{shopId}")]
        public async Task<IActionResult> GetListOrderByShopId(Guid shopId)
        {
            var checkShop = await _userRepository.GetByIdAsync(shopId);
            if (checkShop == null)
            {
                return BadRequest("This shop is not exist");
            }
            
            var listOrder = await _orderRepository.GetBySellerId(shopId);
            return Ok(_mapper.Map<List<OrderReadDTO>>(listOrder));
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDTO order)
        {
            var checkUser = await _userRepository.GetByIdAsync(order.UserId);
            if (checkUser == null)
            {
                return BadRequest("This user is not exist");
            }
            
            var checkProduct = await _productRepository.GetProductByIdAsync(order.ProductId);
            if (checkProduct == null)
            {
                return BadRequest("This product is not exist");
            }
            
            var newOrder = _mapper.Map<Order>(order);   
            newOrder.Id = Guid.NewGuid();   
            newOrder.Status = OrderStatus.Init;
            newOrder.Product = checkProduct;
            newOrder.User = checkUser;
            
            newOrder.ChangeToAcceptState();
            
            await _orderRepository.CreateAsync(newOrder);

            return Ok(_mapper.Map<OrderReadDTO>(newOrder));
        }
        
        [HttpPost("accept")]
        public async Task<IActionResult> ConfirmPayment(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return BadRequest("This order does not exist");
            }
            
            order.ChangeToAcceptState();
            
            await _orderRepository.UpdateAsync(order);
    
            return Ok("Confirm order successfully");
        }
        
        [HttpPost("reject")]
        public async Task<IActionResult> FailedOrder(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return BadRequest("This order does not exist");
            }
            
            order.ChangeToRejectState();
            
            await _orderRepository.UpdateAsync(order);

            return Ok("Mark this order failed successfully");
        }

        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound("This order does not exist");
            }

            await _orderRepository.DeleteAsync(order);
            
            return Ok("Order deleted successfully");
        }

    }
}
