using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskList.Domain.Entities;
using TaskList.Domain.Enums;
using TaskList.Infrastucture.Indentity;

namespace TaskList.Infrastucture.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<TaskItem> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.Priority)
                .HasConversion<string>().IsRequired();

            entity.Property(e => e.Category)
                .HasConversion<string>();

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasDefaultValue(TaskItemStatus.Todo);

            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
