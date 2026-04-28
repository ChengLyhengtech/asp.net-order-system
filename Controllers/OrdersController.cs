using Microsoft.AspNetCore.Mvc;
using aps.net_order_system.Data;
using aps.net_order_system.Queries;
using aps.net_order_system.Commands;
using aps.net_order_system.DTOs;

namespace aps.net_order_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
        {
            var handler = new GetAllOrdersQueryHandler(_context);
            var query = new GetAllOrdersQuery();
            var result = await handler.Handle(query);
            return Ok(result);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var handler = new GetOrderQueryHandler(_context);
            var query = new GetOrderQuery(id);
            var result = await handler.Handle(query);

            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateOrderCommand command)
        {
            var handler = new CreateOrderCommandHandler(_context);
            var newOrderId = await handler.Handle(command);

            // Returns 201 Created and points to the GetById route
            return CreatedAtAction(nameof(GetById), new { id = newOrderId }, new { id = newOrderId });
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusCommand command)
        {
            // Ensure the ID in the URL matches the ID in the command
            command.Id = id;

            var handler = new UpdateOrderStatusCommandHandler(_context);
            var success = await handler.Handle(command);

            if (!success) return NotFound();
            return NoContent();
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteOrderCommand { Id = id };
            var handler = new DeleteOrderCommandHandler(_context);
            var success = await handler.Handle(command);

            if (!success) return NotFound();
            return NoContent();
        }
    }
}