namespace PartsControlSystem.ViewModels
{
    public class DateCodeSearchDto
    {
        public string SearchBy { get; set; } = string.Empty;
        public string Keyword { get; set; } = string.Empty;
    }

    public class DateCodeRowDto
    {
        public int Id { get; set; }
        public string? ControlNo { get; set; }
        public string? MotherMoldCode { get; set; }
        public string? ChildPartcode { get; set; }
        public string? PartName { get; set; }
        public string? Model { get; set; }
        public string? Supplier { get; set; }
        public string? MoldMaker { get; set; }
        public string? Activity { get; set; }
        public string? ReasonOfChange { get; set; }
        public string? Section { get; set; }
        public string? ToolingType { get; set; }
        public string? ToolingCategory { get; set; }
        public DateTime DateImported { get; set; }
    }

    public class UpdatePartCodeDto
    {
        public int Id { get; set; }
        public string NewPartCode { get; set; } = string.Empty;
    }
}