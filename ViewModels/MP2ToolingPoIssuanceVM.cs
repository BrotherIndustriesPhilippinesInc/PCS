namespace PartsControlSystem.ViewModels
{
    public class MP2ToolingPoIssuanceVM : BasedImportData
    {
        public DateTime? LimitDate { get; set; }
        public int RemainingDays { get; set; }
        public DateTime? TargetDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string Remarks { get; set; }
    }
}
