using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace PartsControlSystem.Models
{
    [Table("4m_forms")]
    public class _4mForm
    {
        [Key]
        public int Id { get; set; }

        // Generated in controller → do not require validation
        [StringLength(50)]
        [Column("control_number")]
        public string ControlNumber { get; set; }

        [DataType(DataType.Date)]
        [Column("submission_date")]
        public DateTime SupplierSubmissionDate { get; set; }

        [DataType(DataType.Date)]
        [Column("target_implementation_date")]
        public DateTime TargetImplementationDate { get; set; }

        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(100)]
        [Column("company_name")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Supplier PIC is required")]
        [StringLength(100)]
        [Column("supplier_pic")]
        public string SupplierPIC { get; set; }

        [Required(ErrorMessage = "Type of Change is required")]
        [StringLength(100)]
        [Column("type_of_change")]
        public string TypeOfChange { get; set; }

        [Required(ErrorMessage = "Part Code is required")]
        [StringLength(50)]
        [Column("part_code")]
        public string PartCode { get; set; }

        [Required(ErrorMessage = "Part Name is required")]
        [StringLength(100)]
        [Column("part_name")]
        public string PartName { get; set; }

        [Required(ErrorMessage = "Change Reason is required")]
        [StringLength(250)]
        [Column("change_reason")]
        public string ChangeReason { get; set; }

        [Required(ErrorMessage = "PQC PIC is required")]
        [StringLength(100)]
        [Column("pqc_pic")]
        public string PQCPIC { get; set; }

        // File upload: EF ignores IFormFile
        [NotMapped]
        [Required(ErrorMessage = "Attachment is required")]
        public IFormFile Attachment { get; set; }

        // Save only the file path in the database → generated in controller
        [StringLength(255)]
        [Column("attachment_path")]
        public string AttachmentPath { get; set; }

        [Column("has_reply")]
        public bool HasReply { get; set; } = false;

        [Column("status")]
        public string Status { get; set; }

        [DataType(DataType.Date)]
        [Column("approved_date")]
        public DateTime ApprovedDate { get; set; }

        // New property: flag for email sent
        public bool IsEmailSent { get; set; } = false;

        // NEW PROPERTY FOR PDF
        public string PdfAttachmentPath { get; set; }
    }

}
