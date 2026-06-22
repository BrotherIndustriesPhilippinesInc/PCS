namespace PartsControlSystem.DTO
{
    public class SaveOther4MProcessDto
    {
        public string ControlNumber { get; set; } = string.Empty;
        public string ProcessStep { get; set; } = string.Empty;

        // ── Step 1: Test Run meeting date ──────────────────────────────
        public DateTime? TestRunMeetingTargetDate { get; set; }
        public DateTime? TestRunMeetingActualDate { get; set; }

        // ── Step 2: Kataken Request date ───────────────────────────────
        public DateTime? KatakenRequestTargetDate { get; set; }
        public DateTime? KatakenRequestActualDate { get; set; }

        // ── Step 3: Kataken PH Sample Submission ───────────────────────
        public DateTime? KatakenSampleSubmissionDate { get; set; }

        // ── Step 4: Kataken Evaluation Approval ────────────────────────
        public DateTime? KatakenRequestedDate { get; set; }
        public DateTime? KatakenSubmissionDate { get; set; }
        public DateTime? KatakenApprovedDate { get; set; }
        public string? KatakenStatus { get; set; }
        public string? KatakenRemarks { get; set; }

        // ── Step 5: DE Evaluation ───────────────────────────────────────
        public string? DEReferenceNo { get; set; }
        public string? DEWorkflowSystemNo { get; set; }
        public DateTime? DEPartsReceivedDate { get; set; }
        public DateTime? DEPartsEndorsementDate { get; set; }
        public DateTime? DEActualFinishedDate { get; set; }
        public string? DEEvalStatus { get; set; }
        public string? DERemarks { get; set; }

        // ── Step 6: EE Evaluation ────────────────────────────────────────
        public DateTime? EEPartsReceivedDate { get; set; }
        public DateTime? EEPartsEndorsementDate { get; set; }
        public DateTime? EEActualFinishedDate { get; set; }
        public string? EEEvalStatus { get; set; }
        public string? EERemarks { get; set; }

        // ── Step 7: QA Evaluation (Special QA Evaluation) ────────────────
        public string? QAWorkflowNo { get; set; }
        public DateTime? QATargetDeliveryDate { get; set; }
        public DateTime? QAPartsReceivedDate { get; set; }
        public DateTime? QAPartsEndorsementDate { get; set; }
        public DateTime? QAActualFinishedDate { get; set; }
        public string? QAEvalStatus { get; set; }
        public string? QARemarks { get; set; }

        // ── Step 8: ITF Process ──────────────────────────────────────────
        public DateTime? ITFActualFinishedDate { get; set; }
        public string? ITFStatus { get; set; }
        public string? ITFRemarks { get; set; }

        // ── Step 9: Delivery PO Requisition ──────────────────────────────
        public DateTime? DeliveryPORequestDate { get; set; }
        public DateTime? DeliveryPOIssuanceDate { get; set; }
        public DateTime? DeliveryPOTargetDate { get; set; }

        // ── Step 10: Test Run PO request ─────────────────────────────────
        public DateTime? TestRunPORequestDate { get; set; }
        public DateTime? TestRunPOIssuanceDate { get; set; }

        // ── Step 11: TEST RUN ─────────────────────────────────────────────
        public string? TestRunNo { get; set; }
        public DateTime? TestRunActualReceivedDate { get; set; }
        public DateTime? TestRunActualFinishedDate { get; set; }
        public string? TestResult { get; set; }
        public string? TestRunRemarks { get; set; }

        // ── Step 12: IMPLEMENTATION DATE ─────────────────────────────────
        public DateTime? ImplementationDate { get; set; }

        // ── Step 13: FIRST DELIVERY DATE ──────────────────────────────────
        public DateTime? FirstDeliveryDate { get; set; }
    }
}