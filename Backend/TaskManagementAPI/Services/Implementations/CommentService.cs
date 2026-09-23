using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Exceptions;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories.Interfaces;
using TaskManagementAPI.Services.Interfaces;

namespace TaskManagementAPI.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly ITaskCommentRepository _commentRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;

        public CommentService(
            ITaskCommentRepository commentRepository,
            ITaskRepository taskRepository,
            IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<CommentResponseDto>> GetCommentsAsync(int taskId, int currentUserId, string currentRole)
        {
            var task = await _taskRepository.GetByIdWithTeamAsync(taskId);
            if (task == null) throw new NotFoundException("Task not found.");
            EnsureCanAccessTask(task, currentUserId, currentRole);

            var comments = await _commentRepository.QueryByTask(taskId).ToListAsync();
            return comments.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                TaskItemId = c.TaskItemId,
                UserId = c.UserId,
                UserName = c.User!.FullName,
                Content = c.Content,
                CreatedAt = c.CreatedAt
            });
        }

        public async Task<CommentResponseDto> AddCommentAsync(int taskId, CreateCommentDto dto, int currentUserId, string currentRole)
        {
            var task = await _taskRepository.GetByIdWithTeamAsync(taskId);
            if (task == null) throw new NotFoundException("Task not found.");
            EnsureCanAccessTask(task, currentUserId, currentRole);

            var comment = new TaskComment
            {
                TaskItemId = taskId,
                UserId = currentUserId,
                Content = dto.Content
            };

            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();

            var user = await _userRepository.GetByIdAsync(currentUserId);

            return new CommentResponseDto
            {
                Id = comment.Id,
                TaskItemId = taskId,
                UserId = currentUserId,
                UserName = user!.FullName,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt
            };
        }

        private static void EnsureCanAccessTask(TaskItem task, int currentUserId, string currentRole)
        {
            if (currentRole == "Admin") return;
            if (currentRole == "Manager")
            {
                if (task.Team!.ManagerId == currentUserId) return;
                throw new ForbiddenAccessException("You do not manage this team.");
            }
            if (task.AssignedToId == currentUserId) return;
            throw new ForbiddenAccessException("You cannot access this task.");
        }
    }
}
