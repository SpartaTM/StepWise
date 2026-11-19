
using Microsoft.EntityFrameworkCore;
using StepWise.Models;

namespace StepWise.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Rating>()
            .HasIndex(r => new { r.InstructionId, r.UserId })
            .IsUnique();
    }

    public DbSet<Instruction> Instructions { get; set; }

    public DbSet<Step> Steps { get; set; }

    public DbSet<Progress> Progresses { get; set; }

    public DbSet<ProgressStep> ProgressSteps { get; set; }
    public DbSet<Rating> Ratings { get; set; }
}
