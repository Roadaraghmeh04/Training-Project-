using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManage.DTOs;
using TaskManage.Models;
using TaskManage.Repositories.Interfaces;
using static projectver2.DTOtask;

namespace TaskManage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly AppDbContext _context;

        public UserController(
            IUserRepository userRepository,
            ITaskRepository taskRepository,
            ICategoryRepository categoryRepository,
            AppDbContext context)
        {
            _userRepository = userRepository;
            _taskRepository = taskRepository;
            _categoryRepository = categoryRepository;
            _context = context;
        }


        // ✅ POST Register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email
            };

            var createdUser = await _userRepository.RegisterAsync(user, dto.Password);

            return Ok(new
            {
                message = "User registered successfully",
                createdUser.Id,
                createdUser.Username,
                createdUser.Email
            });
        }

        // ✅ POST Login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _userRepository.LoginAsync(dto.Email, dto.Password);
            if (token == null) return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { token });
        }

        // ✅ GET Profile
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var user = await _userRepository.GetProfileAsync(userId);
            if (user == null) return NotFound(new { message = "User not found" });

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt,
                Categories = user.Categories.Select(c => new { c.Id, c.Name }),
                Tasks = user.TaskEntities.Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    Priority = t.Priority,
                    Status = t.Status,
                    Category = new CategoryDto
                    {
                        Id = t.Category.Id,
                        Name = t.Category.Name
                    }
                })
            });
        }

        // ✅ GET all tasks for current user
        [HttpGet("tasks")]
        [Authorize]
        public async Task<IActionResult> GetMyTasks()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var tasks = await _taskRepository.GetTasksByUserIdAsync(userId);

            return Ok(tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DueDate = t.DueDate,
                Priority = t.Priority,
                Status = t.Status,
                Category = new CategoryDto
                {
                    Id = t.Category.Id,
                    Name = t.Category.Name
                }
            }));
        }


        // ✅ POST add task (uses CategoryName)
        [HttpPost("tasks")]
        [Authorize]
        public async Task<IActionResult> AddTask([FromBody] CreateTaskDto dto)
        {
            try
            {

                int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);


                var category = await _categoryRepository.GetCategoryByNameAsync(userId, dto.CategoryName);
                if (category == null)
                    return BadRequest(new { message = $"Category '{dto.CategoryName}' not found" });


                var task = new TaskEntity
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    DueDate = dto.DueDate,
                    Priority = dto.Priority,
                    Status = dto.Status,
                    CategoryId = category.Id,
                    UserId = userId
                };


                var createdTask = await _taskRepository.AddTaskAsync(userId, task);

                if (createdTask == null)
                    return StatusCode(500, new { message = "Failed to create task" });


                var response = new TaskDto
                {
                    Id = createdTask.Id,
                    Title = createdTask.Title,
                    Description = createdTask.Description,
                    DueDate = createdTask.DueDate,
                    Priority = createdTask.Priority,
                    Status = createdTask.Status,
                    CategoryName = createdTask.Category?.Name ?? category.Name,
                    Category = createdTask.Category != null ? new CategoryDto
                    {
                        Id = createdTask.Category.Id,
                        Name = createdTask.Category.Name,
                        Description = createdTask.Category.Description
                    } : new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {


                return StatusCode(500, new { message = "An error occurred while creating the task" });
            }
        }

        // ✅ PUT update task (uses CategoryName)
        [HttpPut("tasks/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] CreateTaskDto dto)
        {
            try
            {

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }


                if (dto == null)
                {
                    return BadRequest(new { message = "Invalid task data" });
                }


                var task = await _taskRepository.GetTaskByIdAsync(userId, id);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }


                var category = await _categoryRepository.GetCategoryByNameAsync(userId, dto.CategoryName);
                if (category == null)
                {
                    return BadRequest(new { message = "Category not found" });
                }


                task.Title = dto.Title ?? task.Title;
                task.Description = dto.Description ?? task.Description;
                task.DueDate = dto.DueDate ?? task.DueDate;
                task.Priority = dto.Priority ?? task.Priority;
                task.Status = dto.Status ?? task.Status;
                task.CategoryId = category.Id;


                await _taskRepository.UpdateTaskAsync(task);


                return Ok(new TaskDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Priority = task.Priority,
                    Status = task.Status,
                    Category = new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name
                    }
                });
            }
            catch (FormatException)
            {
                return Unauthorized(new { message = "Invalid user ID format in token" });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = "Missing required data", details = ex.ParamName });
            }
            catch (Exception ex)
            {

                Console.WriteLine($"UpdateTask Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // ✅ DELETE task
        [HttpDelete("tasks/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var task = await _taskRepository.GetTaskByIdAsync(userId, id);

            if (task == null) return NotFound(new { message = "Task not found" });

            await _taskRepository.DeleteTaskAsync(task);

            return Ok(new { message = "Task deleted successfully" });
        }

        // GET: api/tasks/my-tasks
        [HttpGet("my-tasks")]
        public async Task<IActionResult> GetMyTasksFiltered(
            [FromQuery] string? status,
            [FromQuery] string? priority,
            [FromQuery] int? categoryId,
            [FromQuery] DateTime? dueDate)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim);


            var query = _context.TaskEntities
                .Where(t => t.UserId == userId)
                .AsQueryable();


            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.Status == status);

            if (!string.IsNullOrEmpty(priority))
                query = query.Where(t => t.Priority == priority);

            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            if (dueDate.HasValue)
                query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDate.Value.Date);

            var tasks = await query.ToListAsync();
            return Ok(tasks);
        }



        // ✅ POST add category
        [HttpPost("categories")]
        [Authorize]
        public async Task<IActionResult> AddCategory([FromBody] CreateCategoryDto dto)
        {
            try
            {

                if (dto == null)
                    return BadRequest(new { message = "Category data is required" });

                if (string.IsNullOrWhiteSpace(dto.Name))
                    return BadRequest(new { message = "Category name is required" });


                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                    return Unauthorized(new { message = "Invalid user authentication" });


                var existingCategory = await _categoryRepository.GetCategoryByNameAsync(userId, dto.Name);
                if (existingCategory != null)
                    return Conflict(new { message = $"Category '{dto.Name}' already exists" });


                var category = new Category
                {
                    Name = dto.Name.Trim(),
                    Description = dto.Description?.Trim() ?? string.Empty
                };


                var createdCategory = await _categoryRepository.AddCategoryAsync(userId, category);

                if (createdCategory == null)
                    return StatusCode(500, new { message = "Failed to create category" });


                var response = new CategoryDto
                {
                    Id = createdCategory.Id,
                    Name = createdCategory.Name,
                    Description = createdCategory.Description
                };

                return CreatedAtAction(
                    nameof(GetCategoryById),
                    new { id = createdCategory.Id },
                    response
                );
            }
            catch (Exception ex)
            {


                return StatusCode(500, new { message = "An error occurred while creating the category" });
            }
        }

        // ✅ GET single category by id
        [HttpGet("categories/{id}")]
        [Authorize]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var category = await _categoryRepository.GetCategoryByIdAsync(userId, id);

            if (category == null)
                return NotFound(new { message = "Category not found" });

            return Ok(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            });
        }




        // ✅ GET all categories
        [HttpGet("categories")]
        [Authorize]
        public async Task<IActionResult> GetMyCategories()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var categories = await _categoryRepository.GetCategoriesByUserIdAsync(userId);

            return Ok(categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }));
        }

        // ✅ PUT update category
        [HttpPut("categories/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var category = await _categoryRepository.GetCategoryByIdAsync(userId, id);
            if (category == null) return NotFound(new { message = "Category not found" });

            category.Name = dto.Name;
            category.Description = dto.Description;

            await _categoryRepository.UpdateCategoryAsync(category);

            return Ok(new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            });
        }

        // ✅ DELETE category
        [HttpDelete("categories/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var category = await _categoryRepository.GetCategoryByIdAsync(userId, id);
            if (category == null) return NotFound(new { message = "Category not found" });

            await _categoryRepository.DeleteCategoryAsync(category);

            return Ok(new { message = "Category deleted successfully" });
        }


        // ✅ Cancel task (Completed -> Cancelled)
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> CancelTask(int id)
        {
            var task = await _context.TaskEntities.FindAsync(id);
            if (task == null) return NotFound("Task not found.");

            if (task.Status == "Completed")
            {
                task.Status = "Cancelled";
                await _context.SaveChangesAsync();
                return Ok(new { task.Id, task.Status });
            }

            return BadRequest("Task is not in Completed status.");
        }

        // ✅ Complete task (InProgress -> Completed)
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> CompleteTask(int id)
        {
            var task = await _context.TaskEntities.FindAsync(id);
            if (task == null) return NotFound("Task not found.");

            if (task.Status == "InProgress")
            {
                task.Status = "Completed";
                await _context.SaveChangesAsync();
                return Ok(new { task.Id, task.Status });
            }

            return BadRequest("Task is not in InProgress status.");
        }

        // ✅ Get tasks sorted by priority
        [Authorize]
        [HttpGet("my-tasks/sorted")]
        public async Task<IActionResult> GetMyTasksSortedByPriority()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);
            var priorityOrder = new List<string> { "High", "Low" };

            var userTasks = await _context.TaskEntities
                .Where(t => t.UserId == userId)
                .ToListAsync();

            var sortedTasks = userTasks
                .OrderBy(t => priorityOrder.IndexOf(t.Priority ?? ""))
                .ToList();

            return Ok(sortedTasks);
        }
    }
}