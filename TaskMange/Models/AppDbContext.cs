using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TaskManage.Models
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<TaskEntity> TaskEntities { get; set; }  

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code.
            => optionsBuilder.UseSqlServer("Data Source=Localhost; Initial Catalog=Datab; Integrated Security=true; TrustServerCertificate=True");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC0751F36E02");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                      .WithMany(p => p.Categories)
                      .HasForeignKey(d => d.UserId)
                      .HasConstraintName("FK__Categorie__UserI__3D5E1FD2");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0700A98CA4");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasMany(e => e.TaskEntities)
                      .WithOne(t => t.User)
                      .HasForeignKey(t => t.UserId)
                      .HasConstraintName("FK__TaskEntity__UserId__3D5E1FD2");

                entity.HasMany(e => e.Categories)
                      .WithOne(c => c.User)
                      .HasForeignKey(c => c.UserId);
            });

            modelBuilder.Entity<TaskEntity>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__TaskEntity__3214EC0775A7B123");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                      .WithMany(p => p.TaskEntities)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK__TaskEntity__UserId__3D5E1FD2");

                entity.HasOne(d => d.Category)
                      .WithMany(p => p.TaskEntities)
                      .HasForeignKey(d => d.CategoryId)
                      .HasConstraintName("FK__TaskEntity__CategoryId__3E52440B");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
