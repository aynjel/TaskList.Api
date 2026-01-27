using TaskList.Domain.Enums;

namespace TaskList.Application.DTOs.Tasks;

public class TaskQueryParameters
{
    public TaskItemStatus? Status { get; set; }
    public PriorityLevel? Priority { get; set; }
    public CategoryType? Category { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
