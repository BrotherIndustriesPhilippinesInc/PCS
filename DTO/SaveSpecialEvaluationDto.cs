namespace PartsControlSystem.DTO
{
    public class SaveSpecialEvaluationDto
    {
        public string ControlNumber { get; set; }
        public string Section { get; set; }
        public string Activity { get; set; }
        public DateTime? LimitDate { get; set; }
        public int RemainingDays { get; set; }
        public string? NeedNoNeed { get; set; }
        public string? LeadTime { get; set; } //No. of days
        public DateTime? ActualFinishDate { get; set; }
        public string? Result { get; set; }
        public string Remarks { get; set; }
    }
}
