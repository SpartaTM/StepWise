namespace StepWise.ViewModels;

// Одна карточка в каталоге
public class InstructionListItemViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int StepsCount { get; set; }
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }
}

// Один шаг на странице инструкции
public class StepViewModel
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

// Страница одной инструкции
public class InstructionDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public List<StepViewModel> Steps { get; set; } = new();
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public int? MyRating { get; set; }
}