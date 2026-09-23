using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.DTOs
{
    public class CreateTeamDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        [Required]
        public int ManagerId { get; set; }
    }

    public class AssignMemberDto
    {
        [Required]
        public int UserId { get; set; }
    }

    public class TeamResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ManagerId { get; set; }
        public string ManagerName { get; set; } = string.Empty;
        public List<TeamMemberDto> Members { get; set; } = new();
    }

    public class TeamMemberDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
