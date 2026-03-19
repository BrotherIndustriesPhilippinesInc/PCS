using PartsControlSystem.Models;

namespace PartsControlSystem.ViewModels
{
    public class ActivityImportViewModel
    {
        public ActivityCardViewModel ActivityCard { get; set; }
        public List<ImportData> ImportDataList { get; set; }
    }
}
