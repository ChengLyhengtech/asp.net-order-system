using aps.net_order_system.Commands.Create;
using aps.net_order_system.Commands.Update;
using aps.net_order_system.Commands.Delete;
using aps.net_order_system.Queries;
using Microsoft.AspNetCore.Mvc;
using aps.net_order_system.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace aps.net_order_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly GetProductHandler _getHandler;
        private readonly GetProductByIdHandler _getByIdHandler;
        private readonly CreateProductCommand _createHandler;
        private readonly UpdateProductHandler _updateHandler;
        private readonly DeleteProductHandler _deleteHandler;
        private readonly GetTopProductHandler _gettophandler;
        private readonly IMediator _mediator;

        public ProductController(
            GetProductHandler getHandler,
            GetProductByIdHandler getByIdHandler,
            CreateProductCommand createHandler,
            UpdateProductHandler updateHandler,
            DeleteProductHandler deleteHandler,
            GetTopProductHandler getTopHandler,
            IMediator mediator)
        {
            _getHandler = getHandler;
            _getByIdHandler = getByIdHandler;
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
            _gettophandler = getTopHandler;
            _mediator = mediator;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetProductQuery query)
        {
            var products = await _getHandler.Handle(query);
            return Ok(products);
        }

        //Get product by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Use the specific ById query and handler
            var product = await _getByIdHandler.HandleAsync(new GetProductByIdQuery { Id = id });
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateDto command)
        {
            try
            {
                var result = await _createHandler.Handle(command);
                // Return 201 Created with the new product data
                return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductCommand command)
        {
            command.Id = id; // Inject the ID from the URL route

            try
            {
                var result = await _updateHandler.HandleAsync(command);
                if (result == null) return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _deleteHandler.HandleAsync(new DeleteProductCommand { Id = id });

            if (!success)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            return NoContent();
        }

        [HttpGet("top")]
        public async Task<IActionResult> TopProduct([FromQuery] int limit = 5, [FromQuery] string sortBy = "qty")
        {
            try
            {
                var query = new GetTopProductQuery { Limit = limit, SortBy = sortBy };
                var result = await _gettophandler.Handle(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id}/availability")]
        //[Authorize(Roles = "Staff, Admin")]
        public async Task<IActionResult> UpdateAvailability(int id, [FromBody] UpdateProductAvailabilityDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new UpdateProductAvailabilityCommand
            {
                Id = id,
                IsAvailable = dto.IsAvailable
            };

            var result = await _mediator.Send(command);

            if (!result)
            {
                return NotFound(new {message = $"Product with ID {id} not found ."});
            }

            return Ok(new { message = $"Product with ID {id} availability updated successfully." });
        }
    }
}