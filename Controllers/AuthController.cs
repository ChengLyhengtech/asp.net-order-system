using aps.net_order_system.DTOs;
using aps.net_order_system.Interface;
using aps.net_order_system.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aps.net_order_system.Controllers
{
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
                    Email = registerDto.Email
                };

                // Create the user with the hashed password
                var createdUser = await _userManager.CreateAsync(userModel, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    // Assign a default role (e.g., "User")
                    var roleResult = await _userManager.AddToRoleAsync(userModel, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(new
                        {
                            UserName = userModel.UserName,
                            Email = userModel.Email,
                            Token = await _tokenService.CreateToken(userModel)
                        });
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Find user by username
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username!");

            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Username not found and/or password incorrect");

            return Ok(new
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user)
            });
        }
    }
}
