using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PartsControlSystem.Models
{
    [Keyless]
    public class ViewMultipleProcurementMonitoring
    {
        public string ControlNumber { get; set; }
        public string? PartName { get; set; }
        public string? Supplier { get; set; }
        public string? Model { get; set; }

        // Step 1: Mold LOA
        public DateTime? mold_loa_target { get; set; }
        public DateTime? mold_loa_actual { get; set; }
        public string? mold_loa_biph_new { get; set; }
        public string? mold_loa_old_biph { get; set; }

        // Step 2: Material LOA
        public DateTime? material_loa_target { get; set; }
        public DateTime? material_loa_actual { get; set; }
        public string? material_loa_status { get; set; }

        // Step 3: Manual FC
        public DateTime? manual_fc_target { get; set; }
        public DateTime? manual_fc_actual { get; set; }

        // Step 4: Tooling PO Issuance
        public DateTime? tooling_po_target { get; set; }
        public DateTime? tooling_po_actual { get; set; }

        // Step 5: Tooling Transfer Date
        public DateTime? tooling_transfer_target { get; set; }
        public DateTime? tooling_transfer_actual { get; set; }

        // Step 6: 4M Application Date
        public DateTime? application_date_target { get; set; }
        public DateTime? application_date_actual { get; set; }

        // Step 7: Kataken PH Sample Submission
        public DateTime? kataken_sub_target { get; set; }
        public DateTime? kataken_sub_actual { get; set; }
        public string? kataken_sub_status { get; set; }

        // Step 8: Kataken PH Sample Approval
        public DateTime? kataken_appr_target { get; set; }
        public DateTime? kataken_appr_actual { get; set; }
        public string? kataken_appr_status { get; set; }

        // Step 9: Availability of Parts Packaging Standard
        public DateTime? pps_target { get; set; }
        public DateTime? pps_approval_date { get; set; }
        public string? pps_available { get; set; }
        public string? pps_packaging_type { get; set; }

        // Step 10: Open Sourcelist
        public DateTime? open_sourcelist_target { get; set; }
        public DateTime? open_sourcelist_actual { get; set; }

        // Step 11: Procurement Type Change
        public DateTime? proc_type_target { get; set; }
        public DateTime? proc_type_change_actual { get; set; }
        public string? proc_type_change_input { get; set; }

        // Step 12: Test Run PO Request Date
        public DateTime? test_run_po_req_target { get; set; }
        public DateTime? test_run_po_req_actual { get; set; }

        // Step 13: Return of Special Procurement Type
        public DateTime? return_special_proc_target { get; set; }
        public DateTime? return_special_proc_actual { get; set; }
        public string? return_special_proc_type { get; set; }

        // Step 14: Test Run PO Date
        public DateTime? test_run_po_target { get; set; }
        public DateTime? test_run_po_actual { get; set; }

        // Step 15: Test Run Delivery Date
        public DateTime? test_run_delivery_target { get; set; }
        public DateTime? test_run_delivery_actual { get; set; }

        // Step 16: Test Run Schedule
        public DateTime? test_run_sched_start { get; set; }
        public DateTime? test_run_sched_finish { get; set; }

        // Step 17: Test Run Approval Date
        public DateTime? test_run_appr_target { get; set; }
        public DateTime? test_run_appr_actual { get; set; }
        public string? test_run_appr_status { get; set; }

        // Step 18: Confirmation of Parts Availability
        public DateTime? confirm_parts_target { get; set; }
        public DateTime? confirm_parts_actual { get; set; }

        // Step 19: Quota Arrangement SAP Input
        public DateTime? quota_target { get; set; }
        public DateTime? quota_actual { get; set; }
        public string? quota_current_supplier { get; set; }
        public string? quota_new_supplier { get; set; }
        public DateTime? quota_input_date { get; set; }

        // Step 20: PO Issuance Date (New Supplier)
        public DateTime? po_issuance_target { get; set; }
        public DateTime? po_issuance_actual { get; set; }

        // Step 21: Parts Delivery Date
        public DateTime? parts_availability_date { get; set; }
        public DateTime? parts_delivery_po_date { get; set; }
        public DateTime? parts_delivery_actual { get; set; }

        // Step 22: Target Usage Date
        public DateTime? target_usage_target { get; set; }
        public DateTime? target_usage_actual { get; set; }
    }
}