using Microsoft.EntityFrameworkCore;
using TaskManage.Models;
using TaskManage.Repositories.Interfaces;


public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    

    public async Task<TaskEntity?> GetTaskByIdAsync(int userId, int taskId)
    {
        return await _context.TaskEntities
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == taskId);
    }

    public async Task<List<TaskEntity>> GetTasksByUserIdAsync(int userId)
    {
        return await _context.TaskEntities
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .ToListAsync();
    }

    public async Task<TaskEntity> AddTaskAsync(int userId, TaskEntity task)
    {
        task.UserId = userId;
        task.CreatedAt = DateTime.UtcNow;

        _context.TaskEntities.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task UpdateTaskAsync(TaskEntity task)
    {
        _context.TaskEntities.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(TaskEntity task)
    {
        _context.TaskEntities.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task<TaskEntity> UpdateStatusAsync(int taskId, string newStatus)
    {
        var taskEntity = await _context.TaskEntities.FindAsync(taskId);
        if (taskEntity == null) return null;

        taskEntity.Status = newStatus;
        await _context.SaveChangesAsync();
        return taskEntity;
    }

    public async Task<List<TaskEntity>> GetTasksAsync(
           int? categoryId = null,
           DateTime? dueDate = null,
           string? status = null,
           string? priority = null)
    {
        var query = _context.TaskEntities
            .Include(t => t.Category)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        if (dueDate.HasValue)
            query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDate.Value.Date);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(t => t.Status != null && t.Status.ToLower() == status.ToLower());

        if (!string.IsNullOrWhiteSpace(priority))
            query = query.Where(t => t.Priority != null && t.Priority.ToLower() == priority.ToLower());

        return await query.ToListAsync();
    }

}
