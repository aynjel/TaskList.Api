namespace TaskList.Application.DTOs.Tasks;

/// <summary>
/// Request to create multiple tasks from extracted document data
/// </summary>
public class CreateFromExtractionRequest
{
    /// <summary>
    /// List of tasks to create (user-selected from extraction results)
    /// </summary>
    public List<ExtractedTaskInput> Tasks { get; set; } = new();
}

/// <summary>
/// Individual task from extraction that user wants to create
/// </summary>
public class ExtractedTaskInput
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int Priority { get; set; } = 2;
    public int Category { get; set; } = 1;
}
