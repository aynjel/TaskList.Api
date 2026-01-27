using TaskList.Domain.Enums;

namespace TaskList.Application.DTOs.Tasks;

public class CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public CategoryType Category { get; set; } = CategoryType.Personal;
}

