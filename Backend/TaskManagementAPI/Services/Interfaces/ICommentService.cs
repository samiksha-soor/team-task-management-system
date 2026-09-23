using TaskManagementAPI.DTOs;

namespace TaskManagementAPI.Services.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentResponseDto>> GetCommentsAsync(int taskId, int currentUserId, string currentRole);
        Task<CommentResponseDto> AddCommentAsync(int taskId, CreateCommentDto dto, int currentUserId, string currentRole);
    }
}
