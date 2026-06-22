namespace PartsControlSystem.ViewModels
{
    public class NewToolingLocalizationProcessVM
    {
        // ── Read-only display fields (from ImportData) ──
        public string ControlNumber { get; set; }
        public string Section { get; set; }
        public string? ToolingType { get; set; }
        public string? ToolingCategory { get; set; }
        public string? Model { get; set; }
        public string? ChildPartcode { get; set; }  // Part Code
        public string? PartName { get; set; }
        public string? ToolingManagement { get; set; }
        public string? Supplier { get; set; }
        public string? BIPHMoldNo { get; set; }
        public string? SupplierMoldNo { get; set; }
        public string? MoldMaker { get; set; }

        // ── Context ──
        public string ActivityName { get; set; }
        public string Category { get; set; }
        public string ProcessStep { get; set; }
        public int StepOrder { get; set; }

        // ── Editable fields ──
        public DateTime? TargetDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string? Result { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Remarks { get; set; }

        // ── Next step ──
        public string? NextProcessStep { get; set; }

        public DateTime? FinalPOIssuedDate { get; set; }
    }
}