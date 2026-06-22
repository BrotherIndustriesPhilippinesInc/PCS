using Microsoft.EntityFrameworkCore;

namespace PartsControlSystem.Models
{
    [Keyless]
    public class ViewSupplierChangeMonitoring
    {
        public string? ControlNumber { get; set; }
        public string? PartName { get; set; }
        public string? Supplier { get; set; }
        public string? Model { get; set; }

        // Step 1: Mold LOA
        public DateTime? mold_loa_target { get; set; }
        public string? mold_loa_biph_new_supplier { get; set; }   // Result = BIPH to New Supplier
        public string? mold_loa_old_supplier_biph { get; set; }   // ReferenceNo = Old Supplier to BIPH

        // Step 2: Material LOA
        public DateTime? material_loa_target { get; set; }
        public string? material_loa_status { get; set; }

        // Step 3: Manual FC to new supplier
        public DateTime? manual_fc_actual { get; set; }

        // Step 4: Tooling PO Issuance
        public DateTime? tooling_po_target { get; set; }

        // Step 5: Tooling Transfer Date
        public DateTime? tooling_transfer_target { get; set; }

        // Step 6: 4M Application Date
        public DateTime? application_date_target { get; set; }

        // Step 7: Kataken PH Sample Submission
        public DateTime? kataken_sub_target { get; set; }
        public DateTime? kataken_sub_actual { get; set; }

        // Step 8: Kataken PH Sample Approval
        public DateTime? kataken_appr_target { get; set; }
        public DateTime? kataken_appr_actual { get; set; }

        // Step 9: DE Sample Received Date
        public DateTime? de_sample_received_actual { get; set; }

        // Step 10: DE Sample Approval
        public DateTime? de_appr_target { get; set; }
        public DateTime? de_appr_actual { get; set; }

        // Step 11: QA Sample Received Date
        public DateTime? qa_sample_received_actual { get; set; }

        // Step 12: QA Sample Approval
        public DateTime? qa_appr_target { get; set; }
        public DateTime? qa_appr_actual { get; set; }

        // Step 13: Availability of Parts Packaging Standard
        public DateTime? pps_target { get; set; }
        public DateTime? pps_approval_date { get; set; }

        // Step 14: Open Sourcelist (New Supplier)
        public DateTime? open_sourcelist_target { get; set; }

        // Step 15: Test Run PO Request Date
        public DateTime? test_run_po_req_target { get; set; }

        // Step 16: Test Run PO Date
        public DateTime? test_run_po_target { get; set; }

        // Step 17: Test Run Delivery Date
        public DateTime? test_run_delivery_actual { get; set; }

        // Step 18: Test Run Schedule
        public DateTime? test_run_sched_start { get; set; }

        // Step 19: Test Run Approval Date
        public DateTime? test_run_appr_target { get; set; }
        public DateTime? test_run_appr_actual { get; set; }

        // Step 20: 4M Approval Date
        public DateTime? approval_date_4m_target { get; set; }

        // Step 21: Request Simulation to MP2
        public DateTime? request_sim_target { get; set; }

        // Step 22: Simulation of Old Suppliers Stocks
        public DateTime? sim_target { get; set; }
        public DateTime? sim_actual_stocks { get; set; }
        public string? sim_shortage_date { get; set; }
        public string? sim_final_po_issued_date { get; set; }

        // Step 23: SAP Setting Change
        public DateTime? sap_setting_target { get; set; }
        public DateTime? sap_setting_actual { get; set; }

        // Step 24: Final PO Delivery (Date)
        public DateTime? final_po_delivery_actual { get; set; }

        // Step 25: BLK and FIX Supplier
        public DateTime? blk_fix_target { get; set; }
        public DateTime? blk_fix_actual { get; set; }

        // Step 26: Recosting Date
        public DateTime? recosting_target { get; set; }
        public DateTime? recosting_actual { get; set; }

        // Step 27: PO Issuance Date (New Supplier)
        public DateTime? po_issuance_actual { get; set; }

        // Step 28: Parts Availability (New Supplier Delivery Date)
        public DateTime? parts_availability_date { get; set; }
        public DateTime? po_delivery_date { get; set; }
        public DateTime? actual_delivery_date { get; set; }

        // Step 29: Target Usage Date
        public DateTime? target_usage_actual { get; set; }
    }
}