using TaskList.Application.DTOs.Tasks;
using TaskList.Domain.Entities;
using TaskList.Domain.Enums;

namespace TaskList.Application.Interfaces;

/// <summary>
/// Repository interface for Task entity operations
/// Abstracts data access logic from business logic
/// </summary>
public interface ITaskRepository
{
    // Query Operations
    Task<TaskItem?> GetByIdAsync(int id, string userId);
    Task<(List<TaskItem> Items, int TotalCount)> GetPagedAsync(string userId, TaskQueryParameters parameters);
    Task<List<TaskItem>> GetAllByUserIdAsync(string userId);
    Task<bool> ExistsAsync(int id, string userId);

    // Command Operations
    Task<TaskItem> AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task DeleteAsync(TaskItem task);
    
    // Save Changes
    Task<int> SaveChangesAsync();
}
