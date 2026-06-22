using Microsoft.EntityFrameworkCore;
namespace PartsControlSystem.Models
{
    [Keyless]
    public class ViewLocalizationMonitoring
    {
        public string? ControlNumber { get; set; }
        public string? PartName { get; set; }
        public string? Supplier { get; set; }
        public string? Model { get; set; }
        // Step 1: Tooling PO Issued Date (MP2-TOOL)
        public DateTime? tooling_po_issued_date { get; set; }
        // Step 2: Drawing Issuance to Supplier (PC-DCI)
        public DateTime? drawing_issuance_date { get; set; }
        public string? drawing_issuance_revision { get; set; }
        // Step 3: Tooling Transfer Date (MP2-TOOL)
        public DateTime? tooling_transfer_target { get; set; }
        // Step 4: 4M Application Date (MP1-PUR)
        public DateTime? application_date_target { get; set; }
        // Step 5: Kataken PH Sample Submission (IQC)
        public DateTime? kataken_sub_target { get; set; }
        public DateTime? kataken_sub_actual { get; set; }
        // Step 6: Kataken PH Sample Approval (IQC)
        public DateTime? kataken_appr_target { get; set; }
        public DateTime? kataken_appr_actual { get; set; }
        // Step 7: Procurement Type Change (PC-DCI)
        public string? proc_type_change_input { get; set; }
        // Step 8: Open Sourcelist Local (MP1-PUR)
        public DateTime? open_sourcelist_target { get; set; }
        // Step 9: Test Run PO Request Date (IQC)
        public DateTime? test_run_po_req_target { get; set; }
        // Step 10: Test Run PO Date (MP2-DOM/OVERSEA)
        public DateTime? test_run_po_target { get; set; }
        // Step 11: Test Run Delivery Date (MP2-DOM/OVERSEA)
        public DateTime? test_run_delivery_actual { get; set; }
        // Step 12: Test Run Schedule (IQC)
        public DateTime? test_run_sched_start { get; set; }
        // Step 13: Test Run Approval Date (IQC)
        public DateTime? test_run_appr_target { get; set; }
        public DateTime? test_run_appr_actual { get; set; }
        // Step 14: 4M Approval Date (IQC)
        public DateTime? approval_date_4m_target { get; set; }
        // Step 15: Request Simulation to MP2 (MP1-PUR)
        public DateTime? request_sim_target { get; set; }
        // Step 16: Simulation of Old Suppliers Stocks (MP2-OVR)
        public DateTime? sim_target { get; set; }
        public DateTime? sim_actual_stocks { get; set; }
        public DateTime? sim_shortage_date { get; set; }
        public DateTime? sim_mrp_transfer_date { get; set; }
        public DateTime? sim_final_po_issued_date { get; set; }
        // Step 17: Final PO Delivery (Date) (MP2-OVR)
        public DateTime? final_po_delivery_actual { get; set; }
        // Step 18: SAP Setting Change (MP1-PUR)
        public DateTime? sap_setting_target { get; set; }
        public DateTime? sap_setting_actual { get; set; }
        // Step 19: BLK and FIX Supplier (MP1-PUR)
        public DateTime? blk_fix_target { get; set; }
        public DateTime? blk_fix_actual { get; set; }
        public string? blk_fix_status { get; set; }
        // Step 20: Recosting Date (MP1-PUR)
        public DateTime? recosting_target { get; set; }
        public DateTime? recosting_actual { get; set; }
        public string? recosting_status { get; set; }
        // Step 21: PO Issuance Date (New Supplier) (MP2-DOM)
        public DateTime? po_issuance_actual { get; set; }
        // Step 22: Parts Availability (New Supplier Delivery Date) (MP2-DOM)
        public DateTime? parts_availability_date { get; set; }
        public DateTime? po_delivery_date { get; set; }
        public DateTime? actual_delivery_date { get; set; }
        // Step 23: Target Usage Date (MP1-PUR)
        public DateTime? target_usage_actual { get; set; }
    }
}