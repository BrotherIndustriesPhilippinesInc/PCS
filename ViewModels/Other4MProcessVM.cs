namespace PartsControlSystem.ViewModels
{
    public class Other4MProcessVM
    {
        public string ControlNumber { get; set; } = string.Empty;
        public string? Section { get; set; }
        public string? Model { get; set; }
        public string? ChildPartcode { get; set; }
        public string? PartName { get; set; }
        public string? Supplier { get; set; }
        public string? BIPHMoldNo { get; set; }
        public string? SupplierMoldNo { get; set; }
        public string? MoldMaker { get; set; }
        public string? ToolingManagement { get; set; }
        public string? ReasonOfChange { get; set; }

        public string ProcessStep { get; set; } = string.Empty;
        public int StepOrder { get; set; }
        public string NextProcessStep { get; set; } = string.Empty;
    }
}