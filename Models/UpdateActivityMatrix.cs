using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsControlSystem.Models
{
    [Table("update_activity_matrix")]
    public class UpdateActivityMatrix
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("section")]
        public string Section { get; set; }

        [Column("activity")]
        public string Activity { get; set; }

        [Column("limit_date")]
        public bool LimitDate { get; set; }

        [Column("remaining_days_until_limit")]
        public bool RemainingDaysUntilLimit { get; set; }

        [Column("transfer_lead_time")]
        public bool TransferLeadTime { get; set; }

        [Column("fabrication_lead_time")]
        public bool FabricationLeadTime { get; set; }

        [Column("need_no_need")]
        public bool NeedOrNoNeed { get; set; }

        [Column("approval_lead_time")]
        public bool ApprovalLeadTime { get; set; }

        [Column("trf_no")]
        public bool TRFNo { get; set; }

        [Column("target")]
        public bool Target { get; set; }

        [Column("actual")]
        public bool Actual { get; set; }

        [Column("result")]
        public bool Result { get; set; }

        [Column("result_email_date_to_supplier")]
        public bool ResultEmailDateToSupplier { get; set; }

        [Column("result_passed_failed")]
        public bool ResultPassedFailed { get; set; }

        [Column("remarks")]
        public bool Remarks { get; set; }

        [Column("parts_shortage_date")]
        public bool PartsShortageDate { get; set; }

        [Column("target_mold_usage_date")]
        public bool TargetMoldUsageDate { get; set; }
    }
}
