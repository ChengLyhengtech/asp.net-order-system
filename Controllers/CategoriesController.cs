using aps.net_order_system.DTOs;
using aps.net_order_system.Queries;
using aps.net_order_system.Commands.Create; // Ensure you import the namespace for your command
using Microsoft.AspNetCore.Mvc;
using aps.net_order_system.Commands.Update;
using aps.net_order_system.Commands.Delete;

namespace aps.net_order_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly GetCategoriesHandler _getHandler;
        private readonly CreateCategoriesCommand _createHandler;
        private readonly UpdateCategoriesHandler _updateHandler;
        private readonly DeleteCategoriesHandler _deleteHandler;

        public CategoriesController(
            GetCategoriesHandler getHandler,
            CreateCategoriesCommand createHandler,
            UpdateCategoriesHandler updateHandler,
            DeleteCategoriesHandler deleteHandler
            )
        {
            _getHandler = getHandler;
            _createHandler = createHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _getHandler.Handle(new GetCategoriesQuery());
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoriesDto command) // 3. Corrected to [FromBody]
        {
            var result = await _createHandler.Handle(command);
            return Ok(result);
        }

        [HttpPut("{id}")] // 4. Added {id} to the route
        public async Task<IActionResult> Update(int id, [FromBody] CategoriesDto dto)
        {
            // 5. Create the command object to pass to the handler
            var command = new UpdateCategoriesCommand
            {
                Id = id,
                ImageUrl = dto.ImageUrl,
                CategoryName = dto.CategoryName
            };

            var result = await _updateHandler.HandleAsync(command);

            return result ? Ok("Updated Successfully") : NotFound();
        }

        [HttpDelete("{id}")] 
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteCategoriesCommand { Id = id };
            var success = await _deleteHandler.HandleAsync(command);
            if (!success)
            {
                return NotFound($"Category {id} not found.");
            }
            return NoContent();
        }
    }
}