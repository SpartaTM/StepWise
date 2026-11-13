
namespace StepWise.Models;

public class ProgressStep
{
    public int Id { get; set; }

    public int ProgressId { get; set; }

    public int StepId { get; set; }

    public bool IsCompleted { get; set; } = false;

    public Progress? Progress { get; set; }

    public Step? Step { get; set; }
}
