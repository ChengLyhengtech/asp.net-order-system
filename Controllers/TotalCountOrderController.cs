using aps.net_order_system.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace aps.net_order_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TotalCountOrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TotalCountOrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("OrderCount")]
        public async Task<IActionResult> GetTotalCount()
        {
            // This triggers the Handler
            var result = await _mediator.Send(new TotalCountOrderQuery());
            return Ok(result);
        }
    }
}
