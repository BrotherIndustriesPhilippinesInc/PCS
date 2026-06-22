using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace PartsControlSystem.Models
{
    [Table("import_data")]
    public class ImportData
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("control_no")]
        public string ControlNo { get; set; }
        [Column("mother_moldcode")]
        public string MotherMoldCode { get; set; }
        [Column("child_partcode")]
        public string ChildPartcode { get; set; }
        [Column("partname")]
        public string PartName { get; set; }
        [Column("model")]
        public string Model { get; set; }
        [Column("supplier")]
        public string Supplier { get; set; }
        [Column("mold_maker")]
        public string MoldMaker { get; set; }
        [Column("supplier_mold_no")]
        public string SupplierMoldNo { get; set; }
        [Column("biph_mold_no")]
        public string BIPHMoldNo { get; set; }
        [Column("tooling_management")]
        public string? ToolingManagement { get; set; }
        [Column("activity")]
        public string? Activity { get; set; }
        [Column("reason_of_change")]
        public string? ReasonOfChange { get; set; }
        [Column("renewal_additional_mold")]
        public string RenewalAdditionalMold { get; set; }
        [Column("new_tooling_localization")]
        public string NewToolingLocalization { get; set; }
        [Column("transfer_tooling")]
        public string TransferTooling { get; set; }
        [Column("change_material")]
        public string ChangeMaterial { get; set; }
        [Column("new_model")]
        public string NewModel { get; set; }
        [Column("non_concurrent")]
        public string NonConcurrent { get; set; }
        [Column("supplier_change_localization")]
        public string SupplierChangeLocalization { get; set; }
        [Column("other_4m")]
        public string Other4M { get; set; }
        [Column("date_imported")]
        public DateTime DateImported { get; set; } = DateTime.UtcNow;

        //New properties - section
        [Column("section")]
        public string Section { get; set; }

        [Column("tooling_type")]
        public string ToolingType { get; set; }

        [Column("tooling_category")]
        public string ToolingCategory { get; set; }

        [Column("multiple_procurement_localization")]
        public string MultipleProcurementLocalization { get; set; }
    }
}
