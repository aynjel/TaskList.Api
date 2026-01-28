using Microsoft.EntityFrameworkCore;
using TaskList.Application.DTOs.Tasks;
using TaskList.Application.Interfaces;
using TaskList.Domain.Entities;

namespace TaskList.Infrastucture.Persistence.Repositories;

/// <summary>
/// Repository implementation for Task entity
/// Handles all data access operations for tasks
/// </summary>
public class TaskRepository(ApplicationDbContext _context) : ITaskRepository
{
    public async Task<TaskItem?> GetByIdAsync(int id, string userId)
    {
        return await _context.TaskItems
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<(List<TaskItem> Items, int TotalCount)> GetPagedAsync(
        string userId, 
        TaskQueryParameters parameters)
    {
        var query = _context.TaskItems
            .Where(t => t.UserId == userId)
            .AsQueryable();

        // Apply filters
        query = ApplyFilters(query, parameters);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = ApplySorting(query, parameters);

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(int id, string userId)
    {
        return await _context.TaskItems
            .AnyAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<List<TaskItem>> GetAllByUserIdAsync(string userId)
    {
        return await _context.TaskItems
            .Where(t => t.UserId == userId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TaskItem> AddAsync(TaskItem task)
    {
        await _context.TaskItems.AddAsync(task);
        return task;
    }

    public Task UpdateAsync(TaskItem task)
    {
        _context.TaskItems.Update(task);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TaskItem task)
    {
        _context.TaskItems.Remove(task);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    #region Private Helper Methods

    private static IQueryable<TaskItem> ApplyFilters(
        IQueryable<TaskItem> query, 
        TaskQueryParameters parameters)
    {
        // Filter by status
        if (parameters.Status.HasValue)
        {
            query = query.Where(t => t.Status == parameters.Status.Value);
        }

        // Filter by priority
        if (parameters.Priority.HasValue)
        {
            query = query.Where(t => t.Priority == parameters.Priority.Value);
        }

        // Filter by category
        if (parameters.Category.HasValue)
        {
            query = query.Where(t => t.Category == parameters.Category.Value);
        }

        // Search in title and description
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(t =>
                t.Title.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                t.Description.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase));
        }

        // Filter by due date range
        if (parameters.DueDateFrom.HasValue)
        {
            query = query.Where(t => t.DueDate >= parameters.DueDateFrom.Value);
        }

        if (parameters.DueDateTo.HasValue)
        {
            query = query.Where(t => t.DueDate <= parameters.DueDateTo.Value);
        }

        return query;
    }

    private static IQueryable<TaskItem> ApplySorting(
        IQueryable<TaskItem> query,
        TaskQueryParameters parameters)
    {
        return parameters.SortBy?.ToLower() switch
        {
            "title" => parameters.SortDescending
                ? query.OrderByDescending(t => t.Title)
                : query.OrderBy(t => t.Title),
            "duedate" => parameters.SortDescending
                ? query.OrderByDescending(t => t.DueDate)
                : query.OrderBy(t => t.DueDate),
            "priority" => parameters.SortDescending
                ? query.OrderByDescending(t => t.Priority)
                : query.OrderBy(t => t.Priority),
            "status" => parameters.SortDescending
                ? query.OrderByDescending(t => t.Status)
                : query.OrderBy(t => t.Status),
            _ => parameters.SortDescending
                ? query.OrderByDescending(t => t.CreatedAt)
                : query.OrderBy(t => t.CreatedAt)
        };
    }

    #endregion
}

