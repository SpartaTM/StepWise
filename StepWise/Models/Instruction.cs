
namespace StepWise.Models;

public class Instruction
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<Step> Steps { get; set; } = new();

    public List<Progress> Progresses { get; set; } = new();
}
