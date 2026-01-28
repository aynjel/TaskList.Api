using TaskList.Application.DTOs.Tasks;

namespace TaskList.Application.DTOs.AI;

public class TaskSummaryResponse
{
    public string Summary { get; set; } = string.Empty;
    public TaskMetrics Metrics { get; set; } = new();
    public List<TaskResponse> TasksToday { get; set; } = new();
    public List<TaskResponse> UpcomingTasks { get; set; } = new();
    public string GeneratedAt { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
}

public class TaskMetrics
{
    public int TotalTasks { get; set; }
    public int DueToday { get; set; }
    public int DueThisWeek { get; set; }
    public int Overdue { get; set; }
    public Dictionary<string, int> ByStatus { get; set; } = new();
    public Dictionary<string, int> ByPriority { get; set; } = new();
    public Dictionary<string, int> ByCategory { get; set; } = new();
}
