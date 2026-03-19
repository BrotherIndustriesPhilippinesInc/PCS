using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsControlSystem.Models
{
    [Table("update_activity_data")]
    public class UpdateActivityData
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("section")]
        [StringLength(50)]
        public string Section { get; set; }

        [Required]
        [Column("activity")]
        [StringLength(100)]
        public string Activity { get; set; }

        [Required]
        [Column("control_no")]
        [StringLength(50)]
        public string ControlNo { get; set; }

        [Column("limit_date")]
        public DateTime? LimitDate { get; set; }

        [Column("remaining_days_until_limit")]
        public int? RemainingDaysUntilLimit { get; set; }

        [Column("transfer_lead_time")]
        public int? TransferLeadTime { get; set; }

        [Column("fabrication_lead_time")]
        public int? FabricationLeadTime { get; set; }

        [Column("need_no_need")]
        [StringLength(20)]
        public string NeedNoNeed { get; set; }

        [Column("approval_lead_time")]
        public int? ApprovalLeadTime { get; set; }

        [Column("trf_no")]
        [StringLength(50)]
        public string TrfNo { get; set; }

        [Column("target")]
        public decimal? Target { get; set; }

        [Column("actual")]
        public decimal? Actual { get; set; }

        [Column("result")]
        [StringLength(50)]
        public string Result { get; set; }

        [Column("result_email_date_to_supplier")]
        public DateTime? ResultEmailDateToSupplier { get; set; }

        [Column("result_passed_failed")]
        [StringLength(20)]
        public string ResultPassedFailed { get; set; }

        [Column("remarks")]
        public string Remarks { get; set; }

        [Column("parts_shortage_date")]
        public DateTime? PartsShortageDate { get; set; }

        [Column("target_mold_usage_date")]
        public DateTime? TargetMoldUsageDate { get; set; }

        [Column("created_by")]
        [StringLength(50)]
        public string CreatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
