using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Services.Interfaces;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private string CurrentRole => User.FindFirstValue(ClaimTypes.Role)!;

        [HttpGet]
        public async Task<IActionResult> GetTeams()
        {
            var teams = await _teamService.GetTeamsAsync(CurrentUserId, CurrentRole);
            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTeam(int id)
        {
            var team = await _teamService.GetTeamAsync(id, CurrentUserId, CurrentRole);
            return Ok(team);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (id, name) = await _teamService.CreateTeamAsync(dto);
            return CreatedAtAction(nameof(GetTeam), new { id }, new { id, name });
        }

        [HttpPost("{id}/members")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddMember(int id, [FromBody] AssignMemberDto dto)
        {
            var message = await _teamService.AddMemberAsync(id, dto, CurrentUserId, CurrentRole);
            return Ok(new { message });
        }

        [HttpDelete("{id}/members/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoveMember(int id, int userId)
        {
            await _teamService.RemoveMemberAsync(id, userId, CurrentUserId, CurrentRole);
            return NoContent();
        }
    }
}
