using System.ComponentModel.DataAnnotations;
namespace StepWise.ViewModels;
public class InstructionFormViewModel
{
   public int? Id { get; set; }
    [Required(ErrorMessage = "Введите название")]
    [StringLength(100, ErrorMessage = "Название не длиннее 100 символов")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите описание")]
    [StringLength(1000,ErrorMessage = "Описание не длиннее 1000 символов")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Выберите категорию")]
    [StringLength(50,ErrorMessage = "Категория не длиннее 50 символов")]
    public string Category { get; set; } = string.Empty;

    public List<StepFormItem> Steps { get; set; } = new();
}
public class StepFormItem
{ 
    public int? Id {  set; get; }
    [StringLength(100,ErrorMessage ="Название шага не длиннее 100 символов")]
    public string? Title { get; set;  }

    [StringLength(1000,ErrorMessage ="Описание шага не длиннее 1000 символов")]
    public string? Description { get; set; }
}

    