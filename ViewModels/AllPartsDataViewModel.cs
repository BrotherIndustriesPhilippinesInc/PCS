namespace PartsControlSystem.Models
{
    public class AllPartsDataViewModel
    {
        public List<AllPartsDataRow> Parts { get; set; } = new();
        public int FinishedCount { get; set; }
        public int OngoingCount { get; set; }
        public int DelayCount { get; set; }
    }

    public class AllPartsDataRow
    {
        public string TransactionNumber { get; set; }
        public string PartName { get; set; }
        public string PartCode { get; set; }       // child_partcode
        public string Supplier { get; set; }
        public string Model { get; set; }
        public string Activity { get; set; }
        public DateTime? InputDate { get; set; }
        public string Status { get; set; }         // "Finished", "Ongoing", "Delay"
        public string CurrentProcess { get; set; }
        public string Remarks { get; set; }
    }
}