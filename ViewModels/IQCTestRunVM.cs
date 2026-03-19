namespace PartsControlSystem.ViewModels
{
    public class IQCTestRunVM : BasedImportData
    {
        public DateTime? LimitDate { get; set; }
        public int RemainingDays { get; set; }
        public DateTime? ActualFinishDate { get; set; }
        public DateTime? ResultEmailDatetoSupplier { get; set; }
        public string? ResultPassedFailed { get; set; } //Passed or Failed
        public string Remarks { get; set; }
    }
}
