using System;

namespace PartsControlSystem.Models
{
    public class ViewOther4MMonitoring
    {
        // ── Identity ──────────────────────────────────────────────────────────
        public string? ControlNumber { get; set; }

        // ── Step 1 : Test Run Meeting Date ────────────────────────────────────
        public DateTime? test_run_meeting_target { get; set; }
        public DateTime? test_run_meeting_actual { get; set; }

        // ── Step 2 : Kataken Request Date ─────────────────────────────────────
        public DateTime? kataken_request_target { get; set; }
        public DateTime? kataken_request_actual { get; set; }

        // ── Step 3 & 4 : Kataken Submission / Evaluation ──────────────────────
        public DateTime? kataken_sample_submission_date { get; set; }
        public DateTime? kataken_requested_date { get; set; }
        public DateTime? kataken_submission_date { get; set; }
        public DateTime? kataken_approved_date { get; set; }
        public string? kataken_status { get; set; }
        public string? kataken_remarks { get; set; }

        // ── Step 5 : DE Evaluation ────────────────────────────────────────────
        public string? de_reference_no { get; set; }
        public string? de_workflow_system_no { get; set; }
        public DateTime? de_parts_received_date { get; set; }
        public DateTime? de_parts_endorsement_date { get; set; }
        public DateTime? de_actual_finished_date { get; set; }
        public string? de_eval_status { get; set; }
        public string? de_remarks { get; set; }

        // ── Step 6 : EE Evaluation ────────────────────────────────────────────
        public DateTime? ee_parts_received_date { get; set; }
        public DateTime? ee_parts_endorsement_date { get; set; }
        public DateTime? ee_actual_finished_date { get; set; }
        public string? ee_eval_status { get; set; }
        public string? ee_remarks { get; set; }

        // ── Step 7 : QA (Special) Evaluation ─────────────────────────────────
        public string? qa_workflow_no { get; set; }
        public DateTime? qa_target_delivery_date { get; set; }
        public DateTime? qa_parts_received_date { get; set; }
        public DateTime? qa_parts_endorsement_date { get; set; }
        public DateTime? qa_actual_finished_date { get; set; }
        public string? qa_eval_status { get; set; }
        public string? qa_remarks { get; set; }

        // ── Step 8 : ITF Process ──────────────────────────────────────────────
        public DateTime? itf_actual_finished_date { get; set; }
        public string? itf_status { get; set; }
        public string? itf_remarks { get; set; }

        // ── Step 9 : Delivery PO Requisition ─────────────────────────────────
        public DateTime? delivery_po_request_date { get; set; }
        public DateTime? delivery_po_issuance_date { get; set; }
        public DateTime? delivery_po_target_date { get; set; }

        // ── Step 10 : Test Run PO Request ────────────────────────────────────
        public DateTime? test_run_po_request_date { get; set; }
        public DateTime? test_run_po_issuance_date { get; set; }

        // ── Step 11 : Test Run ────────────────────────────────────────────────
        public string? test_run_no { get; set; }
        public DateTime? test_run_actual_received_date { get; set; }
        public DateTime? test_run_actual_finished_date { get; set; }
        public string? test_result { get; set; }
        public string? test_run_remarks { get; set; }

        // ── Step 12 : Implementation Date ────────────────────────────────────
        public DateTime? implementation_date { get; set; }

        // ── Step 13 : First Delivery Date ────────────────────────────────────
        public DateTime? first_delivery_date { get; set; }
    }
}