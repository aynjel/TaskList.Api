using System.Text.Json;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Api.Controllers.Common;
using TaskList.Application.DTOs.Tasks;
using TaskList.Application.Interfaces;
using TaskList.Domain.Enums;

namespace TaskList.Api.Controllers.V1;

[ApiVersion("1.0")]
[Authorize]
public class TasksController(
    ITaskService taskService,
    ILogger<TasksController> logger,
    IConfiguration configuration)
    : BaseApiController(logger, configuration)
{
    /// <summary>
    /// Get paginated and filtered list of tasks
    /// </summary>
    /// <param name="parameters">Query parameters for filtering, sorting, and pagination</param>
    /// <returns>List of tasks with pagination metadata in response headers</returns>
    /// <remarks>
    /// Pagination metadata is returned in the X-Pagination response header as a JSON object with the following properties:
    /// - currentPage: Current page number
    /// - pageSize: Number of items per page
    /// - totalCount: Total number of items
    /// - totalPages: Total number of pages
    /// - hasPrevious: Indicates if there is a previous page
    /// - hasNext: Indicates if there is a next page
    /// 
    /// Example: X-Pagination: {"currentPage":1,"pageSize":10,"totalCount":45,"totalPages":5,"hasPrevious":false,"hasNext":true}
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(List<TaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<TaskResponse>>> GetTasks([FromQuery] TaskQueryParameters parameters)
    {
        try
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized<List<TaskResponse>>("Authentication required.");

            var result = await taskService.GetTasksAsync(userId, parameters);
            
            // Create pagination metadata object
            var paginationMetadata = new
            {
                currentPage = result.PageNumber,
                pageSize = result.PageSize,
                totalCount = result.TotalCount,
                totalPages = result.TotalPages,
                hasPrevious = result.HasPreviousPage,
                hasNext = result.HasNextPage
            };
            
            // Serialize to JSON and add to response header
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            
            // Return only the data items in the response body
            return Ok(result.Items);
        }
        catch (Exception ex)
        {
            return HandleException<List<TaskResponse>>(ex, "Failed to retrieve tasks.");
        }
    }

    /// <summary>
    /// Get a specific task by ID
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>Task details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskResponse>> GetTask(int id)
    {
        try
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized<TaskResponse>("Authentication required.");

            var task = await taskService.GetTaskByIdAsync(id, userId);
            
            if (task is null)
                return NotFound<TaskResponse>("Task not found.");

            return Ok(task);
        }
        catch (Exception ex)
        {
            return HandleException<TaskResponse>(ex, "Failed to retrieve task.");
        }
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    /// <param name="request">Task creation data</param>
    /// <returns>Created task</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskResponse>> CreateTask([FromBody] CreateTaskRequest request)
    {
        try
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized<TaskResponse>("Authentication required.");

            var task = await taskService.CreateTaskAsync(userId, request);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            return HandleException<TaskResponse>(ex, "Failed to create task.");
        }
    }

    /// <summary>
    /// Update an existing task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="request">Task update data</param>
    /// <returns>Updated task</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskResponse>> UpdateTask(int id, [FromBody] UpdateTaskRequest request)
    {
        try
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized<TaskResponse>("Authentication required.");

            var task = await taskService.UpdateTaskAsync(id, userId, request);
            
            if (task is null)
                return NotFound<TaskResponse>("Task not found.");

            return Ok(task);
        }
        catch (Exception ex)
        {
            return HandleException<TaskResponse>(ex, "Failed to update task.");
        }
    }

    /// <summary>
    /// Update task status only
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <param name="status">New status</param>
    /// <returns>Updated task</returns>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TaskResponse>> UpdateTaskStatus(int id, [FromBody] TaskItemStatus status)
    {
        try
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized<TaskResponse>("Authentication required.");

            var task = await taskService.UpdateTaskStatusAsync(id, userId, status);
            
            if (task is null)
                return NotFound<TaskResponse>("Task not found.");

            return Ok(task);
        }
        catch (Exception ex)
        {
            return HandleException<TaskResponse>(ex, "Failed to update task status.");
        }
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    /// <param name="id">Task ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Authentication required.");

            var deleted = await taskService.DeleteTaskAsync(id, userId);
            
            if (!deleted)
                return NotFound(new { message = "Task not found." });

            return NoContent();
        }
        catch (Exception ex)
        {
            return HandleException(ex, "Failed to delete task.");
        }
    }

    /// <summary>
    /// Create multiple tasks from document extraction results
    /// </summary>
    /// <param name="request">Selected tasks from extraction to create</param>
    /// <returns>Created tasks and summary</returns>
    /// <remarks>
    /// This endpoint allows users to create tasks in batch after reviewing extracted tasks from a document.
    /// Users can select which tasks to create and edit them before submission.
    /// 
    /// Example request:
    /// {
    ///   "tasks": [
    ///     {
    ///       "title": "Complete API documentation",
    ///       "description": "Write comprehensive API docs",
    ///       "dueDate": "2026-02-15",
    ///       "priority": 3,
    ///       "category": 1
    ///     }
    ///   ]
    /// }
    /// </remarks>
    [HttpPost("create-from-extraction")]
    [ProducesResponseType(typeof(CreateFromExtractionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateFromExtractionResponse>> CreateTasksFromExtraction(
        [FromBody] CreateFromExtractionRequest request)
    {
        try
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized<CreateFromExtractionResponse>("Authentication required.");

            if (request.Tasks == null || request.Tasks.Count == 0)
                return BadRequest(new { message = "No tasks provided for creation." });

            var result = await taskService.CreateTasksFromExtractionAsync(userId, request);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException<CreateFromExtractionResponse>(ex, "Failed to create tasks from extraction.");
        }
    }
}

