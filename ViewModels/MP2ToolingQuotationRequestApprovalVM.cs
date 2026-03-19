namespace PartsControlSystem.ViewModels
{
    public class MP2ToolingQuotationRequestApprovalVM : BasedImportData
    {
        // Approval fields
        public DateTime? LimitDate { get; set; }
        public int RemainingDays { get; set; }
        public DateTime? TargetIssueDate { get; set; }
        public DateTime? ActualIssueDate { get; set; }
        public string Remarks { get; set; }
    }
}
