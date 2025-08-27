using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TaskManage.Models;

[Table("TaskEntity")]
public partial class TaskEntity
{
    [Key]
    public int Id { get; set; }

    [StringLength(200)]
    public string Title { get; set; } = null!;

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    [StringLength(20)]
    public string? Priority { get; set; }

    [StringLength(20)]
    public string? Status { get; set; }

    public int UserId { get; set; }

    public int? CategoryId { get; set; }

    public DateTime? CreatedAt { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("TaskEntities")]
    public virtual Category? Category { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("TaskEntities")]
    public virtual User User { get; set; } = null!;
}
