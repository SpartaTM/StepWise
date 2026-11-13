
namespace StepWise.Models;

public class Progress
{
    public int Id { get; set; }
    public int InstructionId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public bool IsCompleted { get; set; }

    public Instruction? Instruction { get; set; }
    public List<ProgressStep> ProgressSteps { get; set; } = new();
}
