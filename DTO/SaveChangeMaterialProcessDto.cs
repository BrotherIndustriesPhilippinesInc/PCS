namespace PartsControlSystem.DTO
{
    public class SaveChangeMaterialProcessDto
    {
        public string ControlNumber { get; set; }
        public string ProcessStep { get; set; }
        public int StepOrder { get; set; }
        public string NextProcessStep { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string Remarks { get; set; }
    }
}