using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PartsControlSystem.Models
{
    [Keyless]
    [Table("view_change_material_monitoring")]
    public class ViewChangeMaterialMonitoring
    {
        [Column("ControlNumber")]
        public string ControlNumber { get; set; }

        [Column("PartName")]
        public string PartName { get; set; }

        [Column("Model")]
        public string Model { get; set; }

        [Column("Supplier")]
        public string Supplier { get; set; }

        [Column("Section")]
        public string Section { get; set; }

        [Column("ChildPartcode")]
        public string ChildPartcode { get; set; }

        [Column("BIPHMoldNo")]
        public string BIPHMoldNo { get; set; }

        [Column("SupplierMoldNo")]
        public string SupplierMoldNo { get; set; }

        [Column("MoldMaker")]
        public string MoldMaker { get; set; }

        [Column("ToolingManagement")]
        public string ToolingManagement { get; set; }

        // Step 1 — Material LOA (MP1)
        [Column("MaterialLoaTargetDate")]
        public DateTime? MaterialLoaTargetDate { get; set; }

        [Column("MaterialLoaActualDate")]
        public DateTime? MaterialLoaActualDate { get; set; }

        // Step 2 — Kataken PH Sample Submission (IQC)
        [Column("KatakenPhTargetDate")]
        public DateTime? KatakenPhTargetDate { get; set; }

        [Column("KatakenPhActualDate")]
        public DateTime? KatakenPhActualDate { get; set; }

        // Step 3 — Kataken Evaluation Approval (IQC)
        [Column("KatakenEvalTargetDate")]
        public DateTime? KatakenEvalTargetDate { get; set; }

        [Column("KatakenEvalActualDate")]
        public DateTime? KatakenEvalActualDate { get; set; }

        // Step 4 — QA Evaluation (QA)
        [Column("QaEvalTargetDate")]
        public DateTime? QaEvalTargetDate { get; set; }

        [Column("QaEvalActualDate")]
        public DateTime? QaEvalActualDate { get; set; }

        // Step 5 — DE Evaluation (DE)
        [Column("DeEvalTargetDate")]
        public DateTime? DeEvalTargetDate { get; set; }

        [Column("DeEvalActualDate")]
        public DateTime? DeEvalActualDate { get; set; }

        // Step 6 — Test Run (IQC)
        [Column("TestRunTargetDate")]
        public DateTime? TestRunTargetDate { get; set; }

        [Column("TestRunActualDate")]
        public DateTime? TestRunActualDate { get; set; }

        // Step 7 — Implementation Date (MP1)
        [Column("ImplDateTargetDate")]
        public DateTime? ImplDateTargetDate { get; set; }

        [Column("ImplDateActualDate")]
        public DateTime? ImplDateActualDate { get; set; }

        // Step 8 — First Delivery Date (MP1)
        [Column("FirstDelTargetDate")]
        public DateTime? FirstDelTargetDate { get; set; }

        [Column("FirstDelActualDate")]
        public DateTime? FirstDelActualDate { get; set; }

        // Latest current process — drives the STATUS column
        [Column("CurrentProcess")]
        public string? CurrentProcess { get; set; }
    }
}