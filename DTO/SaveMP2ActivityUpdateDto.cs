namespace PartsControlSystem.DTO
{
    public class SaveMP2ActivityUpdateDto
    {
        public string ControlNumber { get; set; }
        public string Section { get; set; }
        public string Activity { get; set; }
        public string? TransferLeadTime { get; set; } //For Tooling Transfer (Arrival in PH) only
        public string? TRFNo { get; set; } //For Tooling Request~Order only
        public DateTime? TargetDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string? Remarks { get; set; }
    }
}
