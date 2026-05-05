using Microsoft.AspNetCore.Mvc;
using aps.net_order_system.Data;
using aps.net_order_system.Queries;
using aps.net_order_system.Commands;
using aps.net_order_system.DTOs;
using MediatR;
using aps.net_order_system.Commands.Create;

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
        private readonly TotalCountOrderHandler _totalCountOrder;
        private readonly IMediator _mediator;
        private readonly GetStaffHistoryHandler _getStaffHistoryHandler;

        public OrdersController(
            GetAllOrdersQueryHandler getAllHandler,
            GetOrderQueryHandler getByIdHandler,
            CreateOrderCommandHandler createHandler,
            UpdateOrderStatusCommandHandler updateHandler,
            DeleteOrderCommandHandler deleteHandler,
            TotalCountOrderHandler totalCountOrder,
            IMediator mediator,
            GetStaffHistoryHandler getStaffHistoryHandler)
        {
            _getAllHandler = getAllHandler;
            _getByIdHandler = getByIdHandler;
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
            _totalCountOrder = totalCountOrder;
            _mediator = mediator;
            _getStaffHistoryHandler = getStaffHistoryHandler;
        }

        [HttpGet]
        public async Task<ActionResult<OrderListResponseDto>> GetAll()
        {
            // 1. Fetch the list of orders
            var orders = await _getAllHandler.Handle(new GetAllOrdersQuery());

            // 2. Fetch the DTO from the total count handler
            var totalCountDto = await _totalCountOrder.Handle(new TotalCountOrderQuery(), HttpContext.RequestAborted);

            // 3. Extract the 'TotalCount' integer from the DTO
            var response = new OrderListResponseDto
            {
                TotalCount = totalCountDto.TotalCount, // Access the property here
                Orders = orders
            };

            return Ok(response);
        }

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

        [HttpPost("manual")]
        public async Task<IActionResult> CreateManualOrder([FromBody] CreateManualOrderCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderId = await _mediator.Send(command);

            return Ok(new
            {
                Id = orderId,
                Message = "Manual order created successfully."
            });
        }

        [HttpGet("history/staff")]
        public async Task<IActionResult> GetStaffHistory(
         [FromQuery] DateTime? from,
         [FromQuery] DateTime? to)
        {
            var result = await _getStaffHistoryHandler.Handle(from, to);
            return Ok(result);
        }
    }
}