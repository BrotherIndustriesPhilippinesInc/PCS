namespace PartsControlSystem.DTO
{
    public class SaveTestRunDto
    {
        public string ControlNumber { get; set; }
        public string Section { get; set; }
        public string Activity { get; set; }
        public DateTime? ActualFinishDate { get; set; }
        public DateTime? ResultEmailDatetoSupplier { get; set; }
        public string? ResultPassedFailed { get; set; } //Passed or Failed
        public string Remarks { get; set; }
    }
}
