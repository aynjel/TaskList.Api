namespace TaskList.Application.DTOs.AI;

public class DocumentExtractionResponse
{
    public List<ExtractedTask> ExtractedTasks { get; set; } = new();
    public List<ExtractedContact> ExtractedContacts { get; set; } = new();
    public string DocumentSummary { get; set; } = string.Empty;
    
    /// <summary>
    /// Natural language AI insights (conversational text for users)
    /// </summary>
    public string AiInsights { get; set; } = string.Empty;
    
    /// <summary>
    /// Structured metadata about the extraction (for analytics and UI components)
    /// </summary>
    public ExtractionMetadata Metadata { get; set; } = new();
    
    public string ProcessedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
}

public class ExtractedTask
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public int Priority { get; set; } = 2;
    public int SuggestedCategory { get; set; } = 1;
    public double Confidence { get; set; }
    public string SourceText { get; set; } = string.Empty;
}

public class ExtractedContact
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Structured metadata about document extraction for analytics and UI
/// </summary>
public class ExtractionMetadata
{
    /// <summary>
    /// Total number of tasks found in the document
    /// </summary>
    public int TotalTasksFound { get; set; }
    
    /// <summary>
    /// Total number of contacts found in the document
    /// </summary>
    public int TotalContactsFound { get; set; }
    
    /// <summary>
    /// Number of tasks with specified due dates
    /// </summary>
    public int TasksWithDueDates { get; set; }
    
    /// <summary>
    /// Number of high-priority tasks (priority = 3)
    /// </summary>
    public int HighPriorityTasks { get; set; }
    
    /// <summary>
    /// Number of medium-priority tasks (priority = 2)
    /// </summary>
    public int MediumPriorityTasks { get; set; }
    
    /// <summary>
    /// Number of low-priority tasks (priority = 1)
    /// </summary>
    public int LowPriorityTasks { get; set; }
    
    /// <summary>
    /// Number of tasks due within the next 7 days
    /// </summary>
    public int UrgentTasks { get; set; }
    
    /// <summary>
    /// Average confidence score of extracted tasks (0.0 - 1.0)
    /// </summary>
    public double AverageConfidence { get; set; }
    
    /// <summary>
    /// Detected document type (e.g., "Meeting Notes", "Email", "Project Plan")
    /// </summary>
    public string DocumentType { get; set; } = "Unknown";
    
    /// <summary>
    /// Processing time in milliseconds
    /// </summary>
    public long ProcessingTimeMs { get; set; }
    
    /// <summary>
    /// Breakdown by suggested category
    /// </summary>
    public Dictionary<string, int> TasksByCategory { get; set; } = new();
}
