using TaskList.Application.DTOs.Tasks;

namespace TaskList.Application.Interfaces;

public interface ITaskService
{
    Task<PagedResult<TaskResponse>> GetTasksAsync(string userId, TaskQueryParameters parameters);
    Task<TaskResponse?> GetTaskByIdAsync(int id, string userId);
    Task<TaskResponse> CreateTaskAsync(string userId, CreateTaskRequest request);
    Task<TaskResponse?> UpdateTaskAsync(int id, string userId, UpdateTaskRequest request);
    Task<bool> DeleteTaskAsync(int id, string userId);
    Task<TaskResponse?> UpdateTaskStatusAsync(int id, string userId, Domain.Enums.TaskItemStatus status);
    
    /// <summary>
    /// Create multiple tasks from extracted document data
    /// </summary>
    Task<CreateFromExtractionResponse> CreateTasksFromExtractionAsync(string userId, CreateFromExtractionRequest request);
}

