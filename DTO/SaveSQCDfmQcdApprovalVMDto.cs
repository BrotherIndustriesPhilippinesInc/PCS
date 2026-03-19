namespace PartsControlSystem.DTO
{
    public class SaveSQCDfmQcdApprovalVMDto
    {
        public string ControlNumber { get; set; }
        public string Section { get; set; }
        public string Activity { get; set; }
        public string? NeedNoNeedQcdMtg { get; set; }
        public string? ApprovalLeadTime { get; set; } //No. of days
        public DateTime? ActualFinishDate { get; set; }
        public string Remarks { get; set; }
    }
}
