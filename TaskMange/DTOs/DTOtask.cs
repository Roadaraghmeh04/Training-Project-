using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using TaskManage.DTOs;
namespace projectver2
{
    public class DTOtask
    {
        public class CreateTaskDto
        {
            public string Title { get; set; } = null!;
            public string? Description { get; set; }
            public DateTime? DueDate { get; set; }
            
            public string? Priority { get; set; }

            public string? Status { get; set; }
            public string? CategoryName { get; set; }
        }
        public class TaskDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = null!;
            public string? Description { get; set; }
            public DateTime? DueDate { get; set; }
            public string? Priority { get; set; }
            public string? Status { get; set; }
            public string? CategoryName { get; set; }
            public CategoryDto? Category { get; set; }
        }
        



    }
}
