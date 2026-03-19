namespace PartsControlSystem.DTO
{
    public class SaveToolingFabricationVMDto
    {
        public string ControlNumber { get; set; }
        public string Section { get; set; }
        public string Activity { get; set; }
        public string? FabricationLeadTime { get; set; } //No. of days
        public DateTime? ActualFinishDate { get; set; }
        public string Remarks { get; set; }
    }
}
