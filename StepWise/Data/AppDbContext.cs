
using Microsoft.EntityFrameworkCore;
using StepWise.Models;

namespace StepWise.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Instruction> Instructions { get; set; }

    public DbSet<Step> Steps { get; set; }

    public DbSet<Progress> Progresses { get; set; }

    public DbSet<ProgressStep> ProgressSteps { get; set; }
}
