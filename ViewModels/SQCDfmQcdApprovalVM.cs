namespace PartsControlSystem.ViewModels
{
    public class SQCDfmQcdApprovalVM : BasedImportData
    {
        public DateTime? LimitDate { get; set; }
        public int RemainingDays { get; set; }
        public string? NeedNoNeedQcdMtg { get; set; }
        public string? ApprovalLeadTime { get; set; } //No. of days
        public DateTime? ActualFinishDate { get; set; }
        public string Remarks { get; set; }

    }
}
