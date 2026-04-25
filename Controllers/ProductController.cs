using aps.net_order_system.Commands.Create;
using aps.net_order_system.Commands.Update;
using aps.net_order_system.Commands.Delete;
using aps.net_order_system.Queries;
using Microsoft.AspNetCore.Mvc;
using aps.net_order_system.DTOs;

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

        public ProductController(
            GetProductHandler getHandler,
            CreateProductCommand createHandler,
            UpdateProductHandler updateHandler,
            DeleteProductHandler deleteHandler)
        {
            _getHandler = getHandler;
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
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
        public async Task<IActionResult> Create([FromBody] ProductDto command)
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
            if (id != command.Id)
            {
                return BadRequest("Product ID mismatch between URL and body.");
            }

            try
            {
                var success = await _updateHandler.HandleAsync(command);
                if (!success) return NotFound($"Product with ID {id} not found.");

                return NoContent(); // 204 Success
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
    }
}