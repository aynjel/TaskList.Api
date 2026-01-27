using TaskList.Domain.Enums;

namespace TaskList.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public PriorityLevel Priority { get; set; }
    public CategoryType Category { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; }
}