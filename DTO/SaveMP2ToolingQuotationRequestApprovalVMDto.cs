namespace PartsControlSystem.DTO
{
    public class SaveMP2ToolingQuotationRequestApprovalVMDto
    {
        public string ControlNumber { get; set; }
        public string Section { get; set; }        
        public string Activity { get; set; }       
        public DateTime? TargetDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string Remarks { get; set; }
    }
}
