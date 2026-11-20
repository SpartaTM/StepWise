namespace StepWise.ViewModels;

public class StepAnalyticsViewModel
{
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DoneCount { get; set; }           // сколько человек отметили шаг
    public int PercentOfStarted { get; set; }    // доля от начавших
    public int DropFromPrevious { get; set; }    // на сколько меньше, чем на прошлом шаге
    public bool IsBottleneck { get; set; }
}

public class InstructionAnalyticsViewModel
{
    public int InstructionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int StartedCount { get; set; }
    public int CompletedCount { get; set; }
    public int CompletionPercent => StartedCount == 0 ? 0 : CompletedCount * 100 / StartedCount;
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public List<StepAnalyticsViewModel> Steps { get; set; } = new();
}