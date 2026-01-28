using TaskList.Application.DTOs.AI;

namespace TaskList.Application.Interfaces;

public interface IAiService
{
    /// <summary>
    /// Generate an AI-powered summary of user's tasks
    /// </summary>
    Task<TaskSummaryResponse> GenerateTaskSummaryAsync(string userId);
    
    /// <summary>
    /// Extract tasks and relevant information from a document
    /// </summary>
    Task<DocumentExtractionResponse> ExtractTasksFromDocumentAsync(Stream documentStream, string fileName, string contentType);
}
