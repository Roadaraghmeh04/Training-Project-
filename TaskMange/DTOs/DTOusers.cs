// 🔹 DTOs for requests
using projectver2;
using System.ComponentModel.DataAnnotations;
using TaskManage.DTOs;
public class RegisterDto
{
    public string Username { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
public class ProfileDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }

    public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
    public List<DTOtask> Tasks { get; set; } = new List<DTOtask>();
}