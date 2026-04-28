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
        // Inject handlers directly
        private readonly GetAllOrdersQueryHandler _getAllHandler;
        private readonly GetOrderQueryHandler _getByIdHandler;
        private readonly CreateOrderCommandHandler _createHandler;
        private readonly UpdateOrderStatusCommandHandler _updateHandler;
        private readonly DeleteOrderCommandHandler _deleteHandler;

        public OrdersController(
            GetAllOrdersQueryHandler getAllHandler,
            GetOrderQueryHandler getByIdHandler,
            CreateOrderCommandHandler createHandler,
            UpdateOrderStatusCommandHandler updateHandler,
            DeleteOrderCommandHandler deleteHandler)
        {
            _getAllHandler = getAllHandler;
            _getByIdHandler = getByIdHandler;
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
            => Ok(await _getAllHandler.Handle(new GetAllOrdersQuery()));

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var result = await _getByIdHandler.Handle(new GetOrderQuery(id));
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateOrderCommand command)
        {
            var newOrderId = await _createHandler.Handle(command);
            return CreatedAtAction(nameof(GetById), new { id = newOrderId }, new { id = newOrderId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusCommand command)
        {
            command.Id = id;
            var success = await _updateHandler.Handle(command);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _deleteHandler.Handle(new DeleteOrderCommand { Id = id });
            return success ? NoContent() : NotFound();
        }
    }
}