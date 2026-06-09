using aps.net_order_system.DTOs;
using aps.net_order_system.Interface;
using aps.net_order_system.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace aps.net_order_system.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController:ControllerBase
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<UserModel> _signInManager;

        public AuthController(UserManager<UserModel> userManager, ITokenService tokenService, SignInManager<UserModel> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }
        // -------------------------------------------------------------
        // 1. PUBLIC REGISTER: Always defaults to "User" role
        // -------------------------------------------------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userModel = new UserModel
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    FullName = registerDto.FullName
                };

                var createdUser = await _userManager.CreateAsync(userModel, registerDto.Password);

                if (!createdUser.Succeeded)
                    return StatusCode(500, createdUser.Errors);

                // Hardcoded fallback to "User" role for public signups
                var roleResult = await _userManager.AddToRoleAsync(userModel, "User");

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(userModel); // Rollback
                    return StatusCode(500, roleResult.Errors);
                }

                return Ok(new
                {
                    userModel.UserName,
                    userModel.Email,
                    Role = "User",
                    Token = await _tokenService.CreateToken(userModel)
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        // -------------------------------------------------------------
        // 2. MANAGEMENT REGISTER: Guarded by Admin, accepts Admin or Staff
        // -------------------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpPost("register-management")]
        public async Task<IActionResult> RegisterManagement([FromBody] RegisterManagementDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Normalize role string casing safely
                string requestedRole = char.ToUpper(dto.Role[0]) + dto.Role.Substring(1).ToLower();

                // Strict check: Only Admin or Staff are permitted here
                if (requestedRole != "Admin" && requestedRole != "Staff")
                {
                    return BadRequest($"Role '{dto.Role}' is unauthorized. Management endpoint only allows: Admin, Staff.");
                }

                var userModel = new UserModel
                {
                    UserName = dto.Username,
                    Email = dto.Email,
                    FullName = dto.FullName
                };

                var createdUser = await _userManager.CreateAsync(userModel, dto.Password);

                if (!createdUser.Succeeded)
                    return StatusCode(500, createdUser.Errors);

                var roleResult = await _userManager.AddToRoleAsync(userModel, requestedRole);

                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(userModel); // Rollback
                    return StatusCode(500, roleResult.Errors);
                }

                return Ok(new
                {
                    userModel.UserName,
                    userModel.Email,
                    Role = requestedRole,
                    Token = await _tokenService.CreateToken(userModel)
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginDto loginDto) // Added [FromBody] for safety
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    // FIX: Use FindByNameAsync so Identity tracks the user model context and roles perfectly
    var user = await _userManager.FindByNameAsync(loginDto.Username);

    if (user == null) return Unauthorized("Invalid username!");

    // Check password
    var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

    if (!result.Succeeded) return Unauthorized("Username not found and/or password incorrect");

    // Fetch roles here just to return them in the response payload for clarity if you want
    var roles = await _userManager.GetRolesAsync(user);

    return Ok(new
    {
        user.UserName, // Automatically names the JSON property "userName" (or "UserName")
        user.Email,
        Roles = roles,   // Keep "Roles =" because the source variable name is "roles" (lowercase)
        Token = await _tokenService.CreateToken(user)
    });
}
      
    }
}
