using System;
using System.Collections.Generic;

namespace PartsControlSystem.ViewModels
{
    // =====================================================================
    // SHARED: base row info shown in all activity tables
    // =====================================================================
    public class IQCMonitoringBaseRow
    {
        public string TransactionNumber { get; set; }
        public string ToolingType { get; set; }
        public string Category { get; set; }
        public string Model { get; set; }
        public string PartCode { get; set; }   // ChildPartcode
        public string PendingItems { get; set; }   // CurrentProcess

        // Returns true only if currently stuck exactly on this step
        public bool IsPendingOn(string stepName) =>
            !string.IsNullOrWhiteSpace(PendingItems) &&
            PendingItems.Trim().Equals(stepName.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    // =====================================================================
    // RENEWAL / ADDITIONAL MOLD
    // IQC-owned steps: Kataken Finish (Local Trial), Kataken Submission (Local Trial), Test Run
    // =====================================================================
    public class IQCRenewalMonitoringRow : IQCMonitoringBaseRow
    {
        // Kataken Finish (Local Trial)
        public DateTime? KatakenFinishTarget { get; set; }
        public DateTime? KatakenFinishActual { get; set; }
        public string? KatakenFinishStatus { get; set; }
        public string? KatakenFinishRemarks { get; set; }

        // Kataken Submission (Local Trial)
        public DateTime? KatakenSubTarget { get; set; }
        public DateTime? KatakenSubActual { get; set; }
        public string? KatakenSubStatus { get; set; }
        public string? KatakenSubRemarks { get; set; }

        // Test Run
        public DateTime? TestRunTarget { get; set; }
        public DateTime? TestRunActual { get; set; }
        public string? TestRunStatus { get; set; }
        public string? TestRunRemarks { get; set; }
    }

    // =====================================================================
    // MULTIPLE PROCUREMENT
    // IQC-owned steps: Kataken PH Sample Submission, Kataken PH Sample Approval,
    //   Availability of Parts Packaging Standard, Test Run PO Request Date,
    //   Test Run Schedule, Test Run Approval Date
    // =====================================================================
    public class IQCMultipleProcurementMonitoringRow : IQCMonitoringBaseRow
    {
        // Step 7: Kataken PH Sample Submission
        public DateTime? KatakenSubTarget { get; set; }
        public DateTime? KatakenSubActual { get; set; }
        public string? KatakenSubStatus { get; set; }
        public string? KatakenSubRemarks { get; set; }

        // Step 8: Kataken PH Sample Approval
        public DateTime? KatakenApprTarget { get; set; }
        public DateTime? KatakenApprActual { get; set; }
        public string? KatakenApprStatus { get; set; }
        public string? KatakenApprRemarks { get; set; }

        // Step 9: Availability of Parts Packaging Standard
        public DateTime? PPSTarget { get; set; }
        public DateTime? PPSActual { get; set; }
        public string? PPSStatus { get; set; }
        public string? PPSRemarks { get; set; }

        // Step 12: Test Run PO Request Date
        public DateTime? TestRunPOReqTarget { get; set; }
        public DateTime? TestRunPOReqActual { get; set; }
        public string? TestRunPOReqStatus { get; set; }
        public string? TestRunPOReqRemarks { get; set; }

        // Step 16: Test Run Schedule
        public DateTime? TestRunSchedStart { get; set; }
        public DateTime? TestRunSchedFinish { get; set; }
        public string? TestRunSchedStatus { get; set; }
        public string? TestRunSchedRemarks { get; set; }

        // Step 17: Test Run Approval Date
        public DateTime? TestRunApprTarget { get; set; }
        public DateTime? TestRunApprActual { get; set; }
        public string? TestRunApprStatus { get; set; }
        public string? TestRunApprRemarks { get; set; }
    }

    // =====================================================================
    // SUPPLIER CHANGE
    // IQC-owned steps: Kataken PH Sample Submission, Kataken PH Sample Approval,
    //   Availability of Parts Packaging Standard, Test Run PO Request Date,
    //   Test Run Schedule, Test Run Approval Date, 4M Approval Date
    // =====================================================================
    public class IQCSupplierChangeMonitoringRow : IQCMonitoringBaseRow
    {
        // Step 7: Kataken PH Sample Submission
        public DateTime? KatakenSubTarget { get; set; }
        public DateTime? KatakenSubActual { get; set; }
        public string? KatakenSubStatus { get; set; }
        public string? KatakenSubRemarks { get; set; }

        // Step 8: Kataken PH Sample Approval
        public DateTime? KatakenApprTarget { get; set; }
        public DateTime? KatakenApprActual { get; set; }
        public string? KatakenApprStatus { get; set; }
        public string? KatakenApprRemarks { get; set; }

        // Step 13: Availability of Parts Packaging Standard
        public DateTime? PPSTarget { get; set; }
        public DateTime? PPSActual { get; set; }
        public string? PPSStatus { get; set; }
        public string? PPSRemarks { get; set; }

        // Step 15: Test Run PO Request Date
        public DateTime? TestRunPOReqTarget { get; set; }
        public DateTime? TestRunPOReqActual { get; set; }
        public string? TestRunPOReqStatus { get; set; }
        public string? TestRunPOReqRemarks { get; set; }

        // Step 18: Test Run Schedule
        public DateTime? TestRunSchedStart { get; set; }
        public DateTime? TestRunSchedFinish { get; set; }
        public string? TestRunSchedStatus { get; set; }
        public string? TestRunSchedRemarks { get; set; }

        // Step 19: Test Run Approval Date
        public DateTime? TestRunApprTarget { get; set; }
        public DateTime? TestRunApprActual { get; set; }
        public string? TestRunApprStatus { get; set; }
        public string? TestRunApprRemarks { get; set; }

        // Step 20: 4M Approval Date
        public DateTime? FourMApprTarget { get; set; }
        public DateTime? FourMApprActual { get; set; }
        public string? FourMApprStatus { get; set; }
        public string? FourMApprRemarks { get; set; }
    }

    // =====================================================================
    // LOCALIZATION (New Tooling / Localization)
    // IQC-owned steps: Kataken PH Sample Submission, Kataken PH Sample Approval,
    //   Test Run PO Request Date, Test Run Schedule, Test Run Approval Date,
    //   4M Approval Date
    // =====================================================================
    public class IQCLocalizationMonitoringRow : IQCMonitoringBaseRow
    {
        // Step 5: Kataken PH Sample Submission
        public DateTime? KatakenSubTarget { get; set; }
        public DateTime? KatakenSubActual { get; set; }
        public string? KatakenSubStatus { get; set; }
        public string? KatakenSubRemarks { get; set; }

        // Step 6: Kataken PH Sample Approval
        public DateTime? KatakenApprTarget { get; set; }
        public DateTime? KatakenApprActual { get; set; }
        public string? KatakenApprStatus { get; set; }
        public string? KatakenApprRemarks { get; set; }

        // Step 9: Test Run PO Request Date
        public DateTime? TestRunPOReqTarget { get; set; }
        public DateTime? TestRunPOReqActual { get; set; }
        public string? TestRunPOReqStatus { get; set; }
        public string? TestRunPOReqRemarks { get; set; }

        // Step 12: Test Run Schedule
        public DateTime? TestRunSchedStart { get; set; }
        public DateTime? TestRunSchedFinish { get; set; }
        public string? TestRunSchedStatus { get; set; }
        public string? TestRunSchedRemarks { get; set; }

        // Step 13: Test Run Approval Date
        public DateTime? TestRunApprTarget { get; set; }
        public DateTime? TestRunApprActual { get; set; }
        public string? TestRunApprStatus { get; set; }
        public string? TestRunApprRemarks { get; set; }

        // Step 14: 4M Approval Date
        public DateTime? FourMApprTarget { get; set; }
        public DateTime? FourMApprActual { get; set; }
        public string? FourMApprStatus { get; set; }
        public string? FourMApprRemarks { get; set; }
    }

    // =====================================================================
    // CHANGE MATERIAL
    // IQC-owned steps: Kataken PH Sample Submission, Kataken Evaluation Approval,
    //   QA Evaluation, DE Evaluation, Test Run
    // =====================================================================
    public class IQCChangeMaterialMonitoringRow : IQCMonitoringBaseRow
    {
        // Step 2: Kataken PH Sample Submission
        public DateTime? KatakenSubTarget { get; set; }
        public DateTime? KatakenSubActual { get; set; }
        public string? KatakenSubStatus { get; set; }
        public string? KatakenSubRemarks { get; set; }

        // Step 3: Kataken Evaluation Approval (IQC)
        public DateTime? KatakenEvalTarget { get; set; }
        public DateTime? KatakenEvalActual { get; set; }
        public string? KatakenEvalStatus { get; set; }
        public string? KatakenEvalRemarks { get; set; }

        // Step 4: QA Evaluation
        public DateTime? QAEvalTarget { get; set; }
        public DateTime? QAEvalActual { get; set; }
        public string? QAEvalStatus { get; set; }
        public string? QAEvalRemarks { get; set; }

        // Step 5: DE Evaluation
        public DateTime? DEEvalTarget { get; set; }
        public DateTime? DEEvalActual { get; set; }
        public string? DEEvalStatus { get; set; }
        public string? DEEvalRemarks { get; set; }

        // Step 6: Test Run
        public DateTime? TestRunTarget { get; set; }
        public DateTime? TestRunActual { get; set; }
        public string? TestRunStatus { get; set; }
        public string? TestRunRemarks { get; set; }
    }

    // =====================================================================
    // OTHER 4M
    // ALL 13 steps are IQC-owned, so every step appears here.
    // =====================================================================
    public class IQCOther4MMonitoringRow : IQCMonitoringBaseRow
    {
        // Step 1: Test Run Meeting Date
        public DateTime? TestRunMeetingTarget { get; set; }
        public DateTime? TestRunMeetingActual { get; set; }
        public string? TestRunMeetingRemarks { get; set; }

        // Step 2: Kataken Request Date
        public DateTime? KatakenRequestTarget { get; set; }
        public DateTime? KatakenRequestActual { get; set; }
        public string? KatakenRequestRemarks { get; set; }

        // Step 3: Kataken PH Sample Submission
        public DateTime? KatakenSubTarget { get; set; }
        public DateTime? KatakenSubActual { get; set; }
        public string? KatakenSubRemarks { get; set; }

        // Step 4: Kataken Evaluation Approval
        public DateTime? KatakenEvalTarget { get; set; }
        public DateTime? KatakenEvalActual { get; set; }
        public string? KatakenEvalStatus { get; set; }
        public string? KatakenEvalRemarks { get; set; }

        // Step 5: DE Evaluation
        public DateTime? DEEvalTarget { get; set; }
        public DateTime? DEEvalActual { get; set; }
        public string? DEEvalStatus { get; set; }
        public string? DEEvalRemarks { get; set; }

        // Step 6: EE Evaluation
        public DateTime? EEEvalTarget { get; set; }
        public DateTime? EEEvalActual { get; set; }
        public string? EEEvalStatus { get; set; }
        public string? EEEvalRemarks { get; set; }

        // Step 7: Special QA Evaluation
        public DateTime? QAEvalTarget { get; set; }
        public DateTime? QAEvalActual { get; set; }
        public string? QAEvalStatus { get; set; }
        public string? QAEvalRemarks { get; set; }

        // Step 8: ITF Process
        public DateTime? ITFTarget { get; set; }
        public DateTime? ITFActual { get; set; }
        public string? ITFStatus { get; set; }
        public string? ITFRemarks { get; set; }

        // Step 9: Delivery PO Requisition
        public DateTime? DeliveryPOTarget { get; set; }
        public DateTime? DeliveryPOActual { get; set; }
        public string? DeliveryPORemarks { get; set; }

        // Step 10: Test Run PO Request
        public DateTime? TestRunPOTarget { get; set; }
        public DateTime? TestRunPOActual { get; set; }
        public string? TestRunPORemarks { get; set; }

        // Step 11: Test Run
        public DateTime? TestRunTarget { get; set; }
        public DateTime? TestRunActual { get; set; }
        public string? TestResult { get; set; }
        public string? TestRunRemarks { get; set; }

        // Step 12: Implementation Date
        public DateTime? ImplementationTarget { get; set; }
        public DateTime? ImplementationActual { get; set; }
        public string? ImplementationRemarks { get; set; }

        // Step 13: First Delivery Date
        public DateTime? FirstDeliveryTarget { get; set; }
        public DateTime? FirstDeliveryActual { get; set; }
        public string? FirstDeliveryRemarks { get; set; }
    }

  

    // =====================================================================
    // WRAPPER passed to the View
    // =====================================================================
    public class IQCMonitoringViewModel
    {
        public List<IQCRenewalMonitoringRow> RenewalRows { get; set; } = new();
        public List<IQCMultipleProcurementMonitoringRow> MultipleProcurementRows { get; set; } = new();
        public List<IQCSupplierChangeMonitoringRow> SupplierChangeRows { get; set; } = new();
        public List<IQCLocalizationMonitoringRow> LocalizationRows { get; set; } = new();
        public List<IQCChangeMaterialMonitoringRow> ChangeMaterialRows { get; set; } = new();
        public List<IQCOther4MMonitoringRow> Other4MRows { get; set; } = new();
    }
}