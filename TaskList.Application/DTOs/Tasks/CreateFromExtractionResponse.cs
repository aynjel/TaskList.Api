namespace TaskList.Application.DTOs.Tasks;

/// <summary>
/// Response after creating tasks from document extraction
/// </summary>
public class CreateFromExtractionResponse
{
    /// <summary>
    /// Successfully created tasks with full details
    /// </summary>
    public List<TaskResponse> CreatedTasks { get; set; } = new();
    
    /// <summary>
    /// Summary of the batch creation operation
    /// </summary>
    public BatchCreationSummary Summary { get; set; } = new();
    
    /// <summary>
    /// Any errors that occurred during creation
    /// </summary>
    public List<TaskCreationError> Errors { get; set; } = new();
}

/// <summary>
/// Summary statistics for batch task creation
/// </summary>
public class BatchCreationSummary
{
    /// <summary>
    /// Total number of tasks submitted for creation
    /// </summary>
    public int TotalSubmitted { get; set; }
    
    /// <summary>
    /// Number of tasks successfully created
    /// </summary>
    public int TotalCreated { get; set; }
    
    /// <summary>
    /// Number of tasks that failed to create
    /// </summary>
    public int TotalFailed { get; set; }
    
    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }
    
    /// <summary>
    /// Timestamp when operation completed
    /// </summary>
    public string CompletedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
}

/// <summary>
/// Information about a task that failed to create
/// </summary>
public class TaskCreationError
{
    /// <summary>
    /// Title of the task that failed
    /// </summary>
    public string TaskTitle { get; set; } = string.Empty;
    
    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// Index in the original request array
    /// </summary>
    public int Index { get; set; }
}
