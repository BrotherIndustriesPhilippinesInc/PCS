namespace PartsControlSystem.ViewModels
{
    public class IQCKatakenSubmissionVM : BasedImportData
    {
        public DateTime? LimitDate { get; set; }
        public int RemainingDays { get; set; }
        public DateTime? ActualSubmissionDate { get; set; }
        public string Remarks { get; set; }

    }
}
