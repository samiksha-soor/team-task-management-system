namespace TaskManagementAPI.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalTasks { get; set; }
        public int ToDoCount { get; set; }
        public int InProgressCount { get; set; }
        public int DoneCount { get; set; }
        public int OverdueCount { get; set; }
        public List<TaskResponseDto> Tasks { get; set; } = new();
    }
}
