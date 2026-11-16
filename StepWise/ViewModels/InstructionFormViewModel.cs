using System.ComponentModel.DataAnnotations;

namespace StepWise.ViewModels;

public class InstructionFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Введите название")]
    [StringLength(100, ErrorMessage = "Не длиннее 100 символов")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите описание")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите категорию")]
    public string Category { get; set; } = string.Empty;

    public List<StepFormItem> Steps { get; set; } = new();
}

public class StepFormItem
{
    public int? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}