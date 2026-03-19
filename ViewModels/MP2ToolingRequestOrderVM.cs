namespace PartsControlSystem.ViewModels
{
    public class MP2ToolingRequestOrderVM : BasedImportData
    {
        public DateTime? LimitDate { get; set; }
        public int RemainingDays { get; set; }
        public string? TransferLeadTime { get; set; } //For Tooling Transfer (Arrival in PH) only
        public string? TRFNo { get; set; } //For Tooling Request~Order only
        public DateTime? TargetDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string Remarks { get; set; }
       
    }
}
