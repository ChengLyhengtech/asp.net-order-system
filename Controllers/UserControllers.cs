using aps.net_order_system.Commands.Create;
using aps.net_order_system.Commands.Delete;
using aps.net_order_system.Commands.Update;
using aps.net_order_system.DTOs;
using aps.net_order_system.Queries; // Add this to access GetUsersHandler
using Microsoft.AspNetCore.Mvc;

namespace aps.net_order_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly GetUsersHandler _getHandler;      // Missing previously
        private readonly UpdateUserHandler _updateHandler;
        private readonly DeleteUserHandler _deleteHandler;

        // 1. Update constructor to inject ALL handlers
        public UserController(
            GetUsersHandler getHandler,
            UpdateUserHandler updateHandler,
            DeleteUserHandler deleteHandler)
        {
            _getHandler = getHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
        }

        // 2. Add the GET method
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _getHandler.Handle(new GetUsersQuery());
            return Ok(users);
        }

       

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            var result = await _updateHandler.HandleAsync(command);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _deleteHandler.HandleAsync(new DeleteUserCommand { Id = id });
            return result ? NoContent() : NotFound();
        }
    }
}