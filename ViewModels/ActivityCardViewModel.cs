namespace PartsControlSystem.ViewModels
{
    public class ActivityCardViewModel
    {
        public string ActivityCode { get; set; }
        public string ActivityName { get; set; }
        public List<SectionSummaryViewModel> SectionCounts { get; set; } = new();
    }
}
