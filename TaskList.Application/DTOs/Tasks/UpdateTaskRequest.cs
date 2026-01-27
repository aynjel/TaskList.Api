using TaskList.Domain.Enums;

namespace TaskList.Application.DTOs.Tasks;

public class UpdateTaskRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public PriorityLevel? Priority { get; set; }
    public CategoryType? Category { get; set; }
    public TaskItemStatus? Status { get; set; }
}

