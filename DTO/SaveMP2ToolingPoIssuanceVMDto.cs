namespace PartsControlSystem.DTO
{
    public class SaveMP2ToolingPoIssuanceVMDto
    {
        public string ControlNumber { get; set; }
        public string Section { get; set; }
        public string Activity { get; set; }
        public DateTime? TargetIssueDate { get; set; }
        public DateTime? ActualIssueDate { get; set; }
        public string Remarks { get; set; }
    }
}
