namespace PartsControlSystem.DTO
{
    public class SaveKatakenSubmissionVMDto
    {
        public string ControlNumber { get; set; }
        public string Section { get; set; }
        public string Activity { get; set; }
        public DateTime? LimitDate { get; set; }
        public int RemainingDays { get; set; }
        public DateTime? ActualSubmissionDate { get; set; }
        public string Remarks { get; set; }
    }
}
