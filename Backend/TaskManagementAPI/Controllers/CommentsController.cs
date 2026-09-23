using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Services.Interfaces;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId}/comments")]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private string CurrentRole => User.FindFirstValue(ClaimTypes.Role)!;

        [HttpGet]
        public async Task<IActionResult> GetComments(int taskId)
        {
            var comments = await _commentService.GetCommentsAsync(taskId, CurrentUserId, CurrentRole);
            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int taskId, [FromBody] CreateCommentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var comment = await _commentService.AddCommentAsync(taskId, dto, CurrentUserId, CurrentRole);
            return CreatedAtAction(nameof(GetComments), new { taskId }, comment);
        }
    }
}
