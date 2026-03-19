namespace PartsControlSystem.ViewModels
{
    public class MP2ToolingFabricationVM : BasedImportData
    {
        public DateTime? LimitDate { get; set; }
        public int RemainingDays { get; set; }
        public string? FabricationLeadTime { get; set; }
        public DateTime? ActualFinishDate { get; set; }
        public string Remarks { get; set; }
    }
}
