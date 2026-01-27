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
    /// <returns>Paginated list of tasks</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<TaskResponse>>> GetTasks([FromQuery] TaskQueryParameters parameters)
    {
        try
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized<PagedResult<TaskResponse>>("Authentication required.");

            var result = await taskService.GetTasksAsync(userId, parameters);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException<PagedResult<TaskResponse>>(ex, "Failed to retrieve tasks.");
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
}
