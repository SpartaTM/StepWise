
namespace StepWise.Models;

public class Step
{
    public int Id { get; set; }
    public int InstructionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Order { get; set; }

    public Instruction? Instruction { get; set; }
}
