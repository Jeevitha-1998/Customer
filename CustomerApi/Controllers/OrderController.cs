using Core.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Get all orders
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        // Get order by id
        [HttpGet("GetOrder/{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        // Get all orders by customer id
        [HttpGet("GetOrdersByCustomer/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomer(int customerId)
        {
            var orders = await _orderService.GetOrdersByCustomerIdAsync(customerId);
            return Ok(orders);
        }

        // Create new order
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _orderService.AddOrderAsync(order);
            return CreatedAtAction(nameof(GetOrder), new { orderId = order.Id }, order);
        }

        // Update existing order
        [HttpPut("UpdateOrder/{orderId}")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] Order order)
        {
            if (orderId != order.Id)
                return BadRequest("Order ID mismatch");

            var existingOrder = await _orderService.GetOrderByIdAsync(orderId);
            if (existingOrder == null)
                return NotFound();

            await _orderService.UpdateOrderAsync(order);
            return NoContent();
        }

        // Delete an order
        [HttpDelete("DeleteOrder/{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
                return NotFound();

            await _orderService.DeleteOrderAsync(orderId);
            return NoContent();
        }

        [HttpGet("GetLastOrderById/{id}")]
        public async Task<IActionResult> GetLastOrderById(int id)
        {
            var orders = await _orderService.GetLastOrderByIdAsync(id);
            return Ok(orders);
        }
    }
}
