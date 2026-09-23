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
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private string CurrentRole => User.FindFirstValue(ClaimTypes.Role)!;

        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] TaskFilterDto filter)
        {
            var tasks = await _taskService.GetTasksAsync(CurrentUserId, CurrentRole, filter);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _taskService.GetTaskAsync(id, CurrentUserId, CurrentRole);
            return Ok(task);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _taskService.CreateTaskAsync(dto, CurrentUserId, CurrentRole);
            return CreatedAtAction(nameof(GetTask), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            await _taskService.UpdateTaskAsync(id, dto, CurrentUserId, CurrentRole);
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTaskStatusDto dto)
        {
            await _taskService.UpdateStatusAsync(id, dto, CurrentUserId, CurrentRole);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _taskService.DeleteTaskAsync(id, CurrentUserId, CurrentRole);
            return NoContent();
        }
    }
}
