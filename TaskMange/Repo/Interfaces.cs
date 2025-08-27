using TaskManage.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskManage.Repositories.Interfaces
{
    public interface IUserRepository
    {
        // User management
        Task<User?> GetProfileAsync(int userId);
        Task<User?> RegisterAsync(User user, string password);
        Task<string?> LoginAsync(string email, string password);
    }

    public interface ITaskRepository
    {

        Task UpdateTaskAsync(TaskEntity task);

        Task<TaskEntity> UpdateStatusAsync(int taskId, string newStatus);
        Task<List<TaskEntity>> GetTasksByUserIdAsync(int userId);
        Task<TaskEntity> AddTaskAsync(int userId, TaskEntity task);

        Task DeleteTaskAsync(TaskEntity task);
        Task<TaskEntity?> GetTaskByIdAsync(int userId, int id);
        Task<List<TaskEntity>> GetTasksAsync(
           int? categoryId = null,
           DateTime? dueDate = null,
           string? status = null,
           string? priority = null
       );
    }

    public interface ICategoryRepository
    {
        Task<Category> AddCategoryAsync(int userId, Category category);
        Task<List<Category>> GetCategoriesByUserIdAsync(int userId);
        Task<Category?> GetCategoryByIdAsync(int userId, int categoryId);
        Task<Category?> GetCategoryByNameAsync(int userId, string categoryName); 
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(Category category);
    }
}
