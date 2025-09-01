using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TaskManage.Models;

[Index("Username", Name = "UQ__Users__536C85E42D4869A6", IsUnique = true)]
[Index("Email", Name = "UQ__Users__A9D1053465C546A0", IsUnique = true)]
public partial class User
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string Username { get; set; } = null!;

    [StringLength(100)]
    public string Email { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    [InverseProperty("User")]
    public virtual ICollection<TaskEntity> TaskEntities { get; set; } = new List<TaskEntity>();
}
