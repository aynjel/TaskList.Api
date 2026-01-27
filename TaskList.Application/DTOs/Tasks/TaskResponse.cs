using TaskList.Domain.Enums;

namespace TaskList.Application.DTOs.Tasks;

public class TaskResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public PriorityLevel Priority { get; set; }
    public CategoryType Category { get; set; }
    public TaskItemStatus Status { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}
