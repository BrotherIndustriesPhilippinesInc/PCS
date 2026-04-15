namespace PartsControlSystem.Models
{
    public class TransactionLogs
    {
        public int Id { get; set; }
        public string TransactionNumber { get; set; }   // ControlNo
        public string PartName { get; set; }   // from import_data
        public string Supplier { get; set; }   // from import_data
        public string Model { get; set; }   // from import_data
        public string Activity { get; set; }   // DetermineActivity() result
        public string Source { get; set; }   // "Import File" or "Update Activity"
        public string PIC { get; set; }   // InputBy from process table or logged-in user
        public DateTime? StartDate { get; set; }   // DateImported from import_data
        public DateTime? EndDate { get; set; }   // latest UpdateAt from activity_current_processes 
        public DateTime? ReceivedDate { get; set; }   // UpdateAt of latest process row (when it moved)
        public DateTime? InputDate { get; set; }   // CreateDate of the latest process-specific table row
        public string CurrentProcess { get; set; }   // latest CurrentProcess value
        public string Status { get; set; }   // "Completed" or "In Progress"
        public string Remarks { get; set; }   // from the latest process-specific table
    }
}
