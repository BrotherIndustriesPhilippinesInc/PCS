namespace PartsControlSystem.ViewModels
{
    public class FourMFormViewModel
    {
        public PartsControlSystem.Models._4mForm DataEntry { get; set; } // single form
        public List<PartsControlSystem.Models._4mForm> FormUpdateList { get; set; } // list of submissions

        // Optional placeholders if you don’t have actual tables
        public List<PartsControlSystem.Models._4mForm> ApprovalList { get; set; } = new List<PartsControlSystem.Models._4mForm>();

        public List<PartsControlSystem.Models._4mForm> SendingList { get; set; } = new List<PartsControlSystem.Models._4mForm>();

    }
}
