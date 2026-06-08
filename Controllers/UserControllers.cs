using aps.net_order_system.Commands.Create;
using aps.net_order_system.Commands.Delete;
using aps.net_order_system.Commands.Update;
using aps.net_order_system.Queries;
using aps.net_order_system.Models; // Ensure you import your UserModel
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using aps.net_order_system.DTOs;

namespace aps.net_order_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // <-- LOCKS DOWN THE ENTIRE CONTROLLER TO ADMINS ONLY
    public class UserController : ControllerBase
    {
        private readonly GetUsersHandler _getHandler;
        private readonly UpdateUserHandler _updateHandler;
        private readonly DeleteUserHandler _deleteHandler;
        private readonly UserManager<UserModel> _userManager; // Inject UserManager for direct password resetting

        public UserController(
            GetUsersHandler getHandler,
            UpdateUserHandler updateHandler,
            DeleteUserHandler deleteHandler,
            UserManager<UserModel> userManager)
        {
            _getHandler = getHandler;
            _updateHandler = updateHandler;
            _deleteHandler = deleteHandler;
            _userManager = userManager;
        }

        // 1. GET ALL USERS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _getHandler.Handle(new GetUsersQuery());
            return Ok(users);
        }

        // 2. EDIT USER NAME, EMAIL, AND FULLNAME
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            var result = await _updateHandler.HandleAsync(command);
            return result ? NoContent() : NotFound();
        }

        // 3. DELETE USER
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _deleteHandler.HandleAsync(new DeleteUserCommand { Id = id });
            return result ? NoContent() : NotFound();
        }

        // 4. ADMIN FORCE RESET PASSWORD
        // Since an Admin is changing this manually behind the scenes, they don't need the user's old password.
        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(string id, [FromBody] AdminResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");

            // Generate a secure reset token behind the scenes
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Forcefully apply the new password using the token
            var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { message = $"Password for user '{user.UserName}' has been reset successfully." });
            }

            return BadRequest(result.Errors);
        }
    }
}