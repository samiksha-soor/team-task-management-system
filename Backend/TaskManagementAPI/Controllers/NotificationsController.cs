using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Services.Interfaces;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var notifications = await _notificationService.GetMyNotificationsAsync(CurrentUserId);
            return Ok(notifications);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id, CurrentUserId);
            return NoContent();
        }
    }
}
