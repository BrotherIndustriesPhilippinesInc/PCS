namespace PartsControlSystem.Models
{
    public class ViewActivityMonitoring
    {
        public string? ControlNumber { get; set; }

        // ===============================
        // MP2 Quotation Request
        // ===============================
        public DateTime? qr_target_date { get; set; }
        public DateTime? qr_actual_date { get; set; }
        public string? qr_remarks { get; set; }

        // ===============================
        // MP2 Request Order
        // ===============================
        public string? tro_trf_no { get; set; }
        public DateTime? tro_target_date { get; set; }
        public DateTime? tro_actual_date { get; set; }
        public string? tro_remarks { get; set; }

        // ===============================
        // MP2 PO Issuance
        // ===============================
        public DateTime? po_target_issue_date { get; set; }
        public DateTime? po_actual_issue_date { get; set; }
        public string? po_remarks { get; set; }

        // ===============================
        // SQC DFM / QCD Approval
        // ===============================
        public string? sqc_need_qcd { get; set; }
        public string? sqc_lead_time { get; set; }
        public DateTime? sqc_finish_date { get; set; }
        public string? sqc_remarks { get; set; }

        // ===============================
        // MP2 Tooling Fabrication
        // ===============================
        public string? fab_lead_time { get; set; }
        public DateTime? fab_finish_date { get; set; }
        public string? fab_remarks { get; set; }

        // ===============================
        // MP2 Tooling Transfer
        // ===============================
        public string? transfer_lead_time { get; set; }
        public DateTime? transfer_target_arrival { get; set; }
        public DateTime? transfer_actual_arrival { get; set; }
        public string? transfer_remarks { get; set; }

        // ===============================
        // IQC Kataken Submission
        // ===============================
        public DateTime? kataken_submission_date { get; set; }
        public string? kataken_submission_remarks { get; set; }

        // ===============================
        // IQC Kataken Finish
        // ===============================
        public DateTime? kataken_finish_date { get; set; }
        public string? kataken_result { get; set; }
        public string? kataken_remarks { get; set; }

        // ===============================
        // DE Evaluation
        // ===============================
        public string? de_need { get; set; }
        public string? de_lead_time { get; set; }
        public DateTime? de_finish_date { get; set; }
        public string? de_result { get; set; }
        public string? de_remarks { get; set; }

        // ===============================
        // QA Special Evaluation
        // ===============================
        public string? qa_need { get; set; }
        public string? qa_lead_time { get; set; }
        public DateTime? qa_finish_date { get; set; }
        public string? qa_result { get; set; }
        public string? qa_remarks { get; set; }

        // ===============================
        // IQC Test Run
        // ===============================
        public DateTime? test_run_finish_date { get; set; }
        public DateTime? test_run_email_date { get; set; }
        public string? test_run_pass_fail { get; set; }
        public string? test_run_remarks { get; set; }

        // ===============================
        // MP2 Capa and PDC
        // ===============================
        public DateTime? target_mold_usage_date { get; set; }
        public DateTime? parts_shortage_date { get; set; }

    }
}
