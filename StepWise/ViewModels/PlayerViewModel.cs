namespace StepWise.ViewModels
{
    public class PlayerStepViewModel
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsDone { get; set; }
    }

    public class PlayerViewModel
    {
        public int InstructionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<PlayerStepViewModel> Steps { get; set; } = new();
        public bool IsCompleted { get; set; }

        public int TotalCount => Steps.Count;
        public int DoneCount => Steps.Count(s => s.IsDone);
        public int Percent => TotalCount == 0 ? 0 : DoneCount * 100 / TotalCount;
    }
}
