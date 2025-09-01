using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TaskManage.Models;

public partial class Category
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    public int UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<TaskEntity> TaskEntities { get; set; } = new List<TaskEntity>();

    [ForeignKey("UserId")]
    [InverseProperty("Categories")]
    public virtual User User { get; set; } = null!;
}
