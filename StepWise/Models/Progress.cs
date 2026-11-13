
namespace StepWise.Models;

public class Progress
{
    public int Id { get; set; }

    public int InstructionId { get; set; }

    public bool IsCompleted { get; set; } = false;

    public DateTime StartedAt { get; set; } = DateTime.Now;

    public Instruction? Instruction { get; set; }

    public List<ProgressStep> ProgressSteps { get; set; } = new();
}
