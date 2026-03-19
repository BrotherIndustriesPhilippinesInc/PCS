namespace PartsControlSystem.ViewModels
{
    public class IQCKatakenFinishVM : BasedImportData
    {
        public DateTime? LimitDate { get; set; }
        public int RemainingDays { get; set; }
        public DateTime? ActualFinishDate { get; set; }
        public string? Result { get; set; }
        public string Remarks { get; set; }
    }
}
