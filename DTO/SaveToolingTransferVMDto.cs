namespace PartsControlSystem.DTO
{
    public class SaveToolingTransferVMDto
    {
        public string ControlNumber { get; set; }
        public string Section { get; set; }
        public string Activity { get; set; }
        public string? TransferLeadTime { get; set; }
        public DateTime? TargetArrivalDate { get; set; }
        public DateTime? ActualArrivalDate { get; set; }
        public string Remarks { get; set; }
    }
}
