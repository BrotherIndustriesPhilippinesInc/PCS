namespace PartsControlSystem.ViewModels
{
    public class MP2ToolingTransferVM : BasedImportData
    {
        public DateTime? LimitDate { get; set; }
        public int RemainingDays { get; set; }
        public string? TransferLeadTime { get; set; }
        public DateTime? TargetArrivalDate { get; set; }
        public DateTime? ActualArrivalDate { get; set; }
        public string Remarks { get; set; }
    }
}
