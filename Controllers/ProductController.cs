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
        private readonly CreateProductCommand _createHandler;
        private readonly UpdateProductHandler _updateHandler;
        private readonly DeleteProductHandler _deleteHandler;
        private readonly GetTopProductHandler _gettophandler;
        private readonly IMediator _mediator;

        public ProductController(
            GetProductHandler getHandler,
            CreateProductCommand createHandler,
            UpdateProductHandler updateHandler,
            DeleteProductHandler deleteHandler,
            GetTopProductHandler getTopHandler,
            IMediator mediator)
        {
            _getHandler = getHandler;
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
            _gettophandler = getTopHandler;
            _mediator = mediator;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _getHandler.Handle(new GetProductQuery());
            return Ok(products);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto command)
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

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductCommand command)
        {
            command.Id = id;

            try
            {
                var result = await _updateHandler.HandleAsync(command);
                if (result == null)
                    return NotFound();
                return Ok(result); // ✅ return updated product with id
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
        public async Task<IActionResult> TopProduct([FromQuery] int limit = 5)
        {
            try
            {
                var query = new GetTopProductQuery { Limit = limit };
                var result = await _gettophandler.Handle(query); // Use the field here
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