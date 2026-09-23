using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Models;
using TaskManagementAPI.Services.Interfaces;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        [HttpPatch("{id}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] UserRole role)
        {
            var user = await _userService.UpdateRoleAsync(id, role);
            return Ok(user);
        }
    }
}
