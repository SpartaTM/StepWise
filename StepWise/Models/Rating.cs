namespace StepWise.Models;

public class Rating
{
    public int Id { get; set; }
    public int InstructionId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int Value { get; set; }   // от 1 до 5
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Instruction? Instruction { get; set; }
}