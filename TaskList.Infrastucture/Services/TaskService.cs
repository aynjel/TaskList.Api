using Microsoft.Extensions.Logging;
using TaskList.Application.DTOs.Tasks;
using TaskList.Application.Interfaces;
using TaskList.Domain.Entities;
using TaskList.Domain.Enums;

namespace TaskList.Infrastucture.Services;

/// <summary>
/// Service for task business logic
/// Uses repository for data access - follows Repository Pattern
/// </summary>
public class TaskService(ITaskRepository _taskRepository, ILogger<TaskService> _logger) : ITaskService
{
    public async Task<PagedResult<TaskResponse>> GetTasksAsync(string userId, TaskQueryParameters parameters)
    {
        _logger.LogInformation("Fetching tasks for user {UserId} with parameters: {@Parameters}", userId, parameters);

        var (items, totalCount) = await _taskRepository.GetPagedAsync(userId, parameters);

        var taskResponses = items.Select(MapToTaskResponse).ToList();

        _logger.LogInformation("Retrieved {Count} tasks out of {TotalCount} for user {UserId}",
            taskResponses.Count, totalCount, userId);

        return new PagedResult<TaskResponse>
        {
            Items = taskResponses,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    public async Task<TaskResponse?> GetTaskByIdAsync(int id, string userId)
    {
        _logger.LogInformation("Fetching task {TaskId} for user {UserId}", id, userId);

        var task = await _taskRepository.GetByIdAsync(id, userId);

        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} not found for user {UserId}", id, userId);
            return null;
        }

        return MapToTaskResponse(task);
    }

    public async Task<TaskResponse> CreateTaskAsync(string userId, CreateTaskRequest request)
    {
        _logger.LogInformation("Creating task for user {UserId}: {Title}", userId, request.Title);

        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description ?? string.Empty,
            DueDate = request.DueDate,
            Priority = request.Priority,
            Category = request.Category,
            Status = TaskItemStatus.Todo,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _taskRepository.AddAsync(task);
        await _taskRepository.SaveChangesAsync();

        _logger.LogInformation("Task created successfully with ID {TaskId}", task.Id);

        return MapToTaskResponse(task);
    }

    public async Task<TaskResponse?> UpdateTaskAsync(int id, string userId, UpdateTaskRequest request)
    {
        _logger.LogInformation("Updating task {TaskId} for user {UserId}", id, userId);

        var task = await _taskRepository.GetByIdAsync(id, userId);

        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} not found for user {UserId}", id, userId);
            return null;
        }

        // Update only provided fields
        if (request.Title is not null)
        {
            task.Title = request.Title;
        }

        if (request.Description is not null)
        {
            task.Description = request.Description;
        }

        if (request.DueDate.HasValue)
        {
            task.DueDate = request.DueDate;
        }

        if (request.Priority.HasValue)
        {
            task.Priority = request.Priority.Value;
        }

        if (request.Category.HasValue)
        {
            task.Category = request.Category.Value;
        }

        if (request.Status.HasValue)
        {
            task.Status = request.Status.Value;
        }

        task.LastModifiedAt = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task);
        await _taskRepository.SaveChangesAsync();

        _logger.LogInformation("Task {TaskId} updated successfully", id);

        return MapToTaskResponse(task);
    }

    public async Task<bool> DeleteTaskAsync(int id, string userId)
    {
        _logger.LogInformation("Deleting task {TaskId} for user {UserId}", id, userId);

        var task = await _taskRepository.GetByIdAsync(id, userId);

        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} not found for user {UserId}", id, userId);
            return false;
        }

        await _taskRepository.DeleteAsync(task);
        await _taskRepository.SaveChangesAsync();

        _logger.LogInformation("Task {TaskId} deleted successfully", id);

        return true;
    }

    public async Task<TaskResponse?> UpdateTaskStatusAsync(int id, string userId, TaskItemStatus status)
    {
        _logger.LogInformation("Updating task {TaskId} status to {Status} for user {UserId}",
            id, status, userId);

        var task = await _taskRepository.GetByIdAsync(id, userId);

        if (task is null)
        {
            _logger.LogWarning("Task {TaskId} not found for user {UserId}", id, userId);
            return null;
        }

        task.Status = status;
        task.LastModifiedAt = DateTime.UtcNow;

        await _taskRepository.UpdateAsync(task);
        await _taskRepository.SaveChangesAsync();

        _logger.LogInformation("Task {TaskId} status updated to {Status}", id, status);

        return MapToTaskResponse(task);
    }

    public async Task<CreateFromExtractionResponse> CreateTasksFromExtractionAsync(
        string userId, 
        CreateFromExtractionRequest request)
    {
        _logger.LogInformation("Creating {Count} tasks from extraction for user {UserId}", 
            request.Tasks.Count, userId);

        var startTime = DateTime.UtcNow;
        var createdTasks = new List<TaskResponse>();
        var errors = new List<TaskCreationError>();

        for (int i = 0; i < request.Tasks.Count; i++)
        {
            var extractedTask = request.Tasks[i];
            
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(extractedTask.Title))
                {
                    errors.Add(new TaskCreationError
                    {
                        Index = i,
                        TaskTitle = extractedTask.Title,
                        ErrorMessage = "Task title is required"
                    });
                    continue;
                }

                // Create the task
                var task = new TaskItem
                {
                    Title = extractedTask.Title,
                    Description = extractedTask.Description ?? string.Empty,
                    DueDate = extractedTask.DueDate,
                    Priority = (PriorityLevel)extractedTask.Priority,
                    Category = (CategoryType)extractedTask.Category,
                    Status = TaskItemStatus.Todo,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                await _taskRepository.AddAsync(task);
                await _taskRepository.SaveChangesAsync();

                createdTasks.Add(MapToTaskResponse(task));
                
                _logger.LogInformation("Task created from extraction: {TaskId} - {Title}", 
                    task.Id, task.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task from extraction at index {Index}: {Title}", 
                    i, extractedTask.Title);
                
                errors.Add(new TaskCreationError
                {
                    Index = i,
                    TaskTitle = extractedTask.Title,
                    ErrorMessage = ex.Message
                });
            }
        }

        var processingTime = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

        _logger.LogInformation(
            "Batch creation completed: {Created} created, {Failed} failed out of {Total} tasks", 
            createdTasks.Count, errors.Count, request.Tasks.Count);

        return new CreateFromExtractionResponse
        {
            CreatedTasks = createdTasks,
            Errors = errors,
            Summary = new BatchCreationSummary
            {
                TotalSubmitted = request.Tasks.Count,
                TotalCreated = createdTasks.Count,
                TotalFailed = errors.Count,
                ProcessingTimeMs = processingTime
            }
        };
    }

    private static TaskResponse MapToTaskResponse(TaskItem task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Priority = task.Priority,
            Category = task.Category,
            Status = task.Status,
            UserId = task.UserId,
            CreatedAt = task.CreatedAt,
            LastModifiedAt = task.LastModifiedAt
        };
    }
}
