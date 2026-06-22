using PartsControlSystem.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartsControlSystem.Models
{
    [Table("new_tooling_process_mapping")]
    public class NewToolingProcessMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// "Multiple Procurement" | "Supplier Change" | "Localization"
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Category { get; set; }

        public int StepOrder { get; set; }

        [Required]
        [StringLength(200)]
        public string ProcessStep { get; set; }

        /// <summary>
        /// Section responsible for this step
        /// e.g. "MP1-PUR", "MP2-DOM", "IQC", "DE", "QA", "PDC-Loc", "PC-DCI", "PUR-CTRL", "MP2-TOOL"
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Section { get; set; }

        [Column("LeadTimeDays")]
        public decimal LeadTimeDays { get; set; } = 0;
    }
}


namespace PartsControlSystem.Data
{
    public static class NewToolingProcessMappingSeed
    {
        public static NewToolingProcessMapping[] GetSeedData()
        {
            int id = 1;

            var rows = new List<(string cat, int order, string step, string section)>
            {
                // ── MULTIPLE PROCUREMENT (22 steps) ──────────────────────────
                ("Multiple Procurement",  1,  "Mold LOA",                                                        "MP1-PUR"),
                ("Multiple Procurement",  2,  "Material LOA",                                                    "MP1-PUR"),
                ("Multiple Procurement",  3,  "Manual FC to new supplier",                                       "MP2-DOM"),
                ("Multiple Procurement",  4,  "Tooling PO issuance",                                            "MP2-TOOLING"),
                ("Multiple Procurement",  5,  "Tooling Transfer Date",                                          "MP2-TOOLING"),
                ("Multiple Procurement",  6,  "4M Application Date",                                            "MP1-PUR"),
                ("Multiple Procurement",  7,  "Kataken PH Sample Submission",                                   "IQC"),
                ("Multiple Procurement",  8,  "Kataken PH Sample Approval",                                     "IQC"),
                ("Multiple Procurement",  9,  "Availability of Parts Packaging Standard",                       "IQC"),
                ("Multiple Procurement", 10,  "Open sourcelist (New Supplier) / Updated Price input",           "MP1-PUR"),
                ("Multiple Procurement", 11,  "Procurement Type Change",                                        "MP1-PUR"),
                ("Multiple Procurement", 12,  "Test Run PO Request Date",                                       "IQC"),
                ("Multiple Procurement", 13,  "Return of Special procurement type",                             "MP1-PUR"),
                ("Multiple Procurement", 14,  "Test Run PO Date",                                               "MP2-DOM"),
                ("Multiple Procurement", 15,  "Test Run Delivery Date",                                         "MP2-DOM"),
                ("Multiple Procurement", 16,  "Test Run Schedule",                                              "IQC"),
                ("Multiple Procurement", 17,  "Test Run Approval Date",                                         "IQC"),
                ("Multiple Procurement", 18,  "Confirmation of Parts Availability",                             "MP1-PUR"),
                ("Multiple Procurement", 19,  "Quota Arrangement SAP input",                                    "MP1-PUR"),
                ("Multiple Procurement", 20,  "PO issuance Date (New Supplier)",                                "MP2-DOM"),
                ("Multiple Procurement", 21,  "Parts Delivery Date (New Supplier Delivery Date)",               "MP2-DOM"),
                ("Multiple Procurement", 22,  "Target Usage Date",                                              "MP2"),
 
                // ── SUPPLIER CHANGE (29 steps) ────────────────────────────────
                ("Supplier Change",  1,  "Mold LOA",                                                            "MP1-PUR"),
                ("Supplier Change",  2,  "Material LOA",                                                        "MP1-PUR"),
                ("Supplier Change",  3,  "Manual FC to new supplier",                                           "MP2-DOM"),
                ("Supplier Change",  4,  "Tooling PO issuance",                                                 "MP2-TOOLING"),
                ("Supplier Change",  5,  "Tooling Transfer Date",                                               "MP2-TOOLING"),
                ("Supplier Change",  6,  "4M Application Date",                                                 "MP1-PUR"),
                ("Supplier Change",  7,  "Kataken PH Sample Submission",                                        "IQC"),
                ("Supplier Change",  8,  "Kataken PH Sample Approval",                                          "IQC"),
                ("Supplier Change",  9,  "DE Sample Received Date",                                             "DE"),
                ("Supplier Change", 10,  "DE Sample Approval",                                                  "DE"),
                ("Supplier Change", 11,  "QA Sample Received Date",                                             "QA"),
                ("Supplier Change", 12,  "QA Sample Approval",                                                  "QA"),
                ("Supplier Change", 13,  "Availability of Parts Packaging Standard",                            "IQC"),
                ("Supplier Change", 14,  "Open sourcelist (New Supplier)",                                      "MP1-PUR"),
                ("Supplier Change", 15,  "Test Run PO Request Date",                                            "IQC"),
                ("Supplier Change", 16,  "Test Run PO Date",                                                    "MP2-DOM"),
                ("Supplier Change", 17,  "Test Run Delivery Date",                                              "MP2-DOM"),
                ("Supplier Change", 18,  "Test Run Schedule",                                                   "IQC"),
                ("Supplier Change", 19,  "Test Run Approval Date",                                              "IQC"),
                ("Supplier Change", 20,  "4M Approval Date",                                                    "IQC"),
                ("Supplier Change", 21,  "Request Simulation to MP2",                                           "MP1-PUR"),
                ("Supplier Change", 22,  "Simulation of Old Suppliers Stocks",                                  "MP2-DOM"),
                ("Supplier Change", 23,  "Final PO Delivery (Date)",                                            "MP2-DOM"),
                ("Supplier Change", 24,  "SAP Setting Change",                                                  "MP1-PUR"),
                ("Supplier Change", 25,  "BLK and FIX Supplier",                                               "MP1-PUR"),
                ("Supplier Change", 26,  "Recosting Date",                                                      "MP1-PUR"),
                ("Supplier Change", 27,  "PO issuance Date (New Supplier)",                                     "MP2-DOM"),
                ("Supplier Change", 28,  "Parts Availability (New Supplier Delivery Date)",                     "MP2-DOM"),
                ("Supplier Change", 29,  "Target Usage Date",                                                   "MP2"),
 
                // ── LOCALIZATION (23 steps) ───────────────────────────────────
                ("Localization",  1,  "Tooling PO Issued Date",                                                 "MP2-TOOL"),
                ("Localization",  2,  "Drawing Issuance to Supplier",                                           "PC-DCI"),
                ("Localization",  3,  "Tooling Transfer Date",                                                  "MP2-TOOL"),
                ("Localization",  4,  "4M Application Date",                                                    "MP1-PUR"),
                ("Localization",  5,  "Kataken PH Sample Submission",                                           "IQC"),
                ("Localization",  6,  "Kataken PH Sample Approval",                                             "IQC"),
                ("Localization",  7,  "Procurement Type Change",                                                "PC-DCI"),
                ("Localization",  8,  "Open Sourcelist Local",                                                  "MP1-PUR"),
                ("Localization",  9,  "Test Run PO Request Date",                                               "IQC"),
                ("Localization", 10,  "Test Run PO Date",                                                       "MP2-DOM"),
                ("Localization", 11,  "Test Run Delivery Date",                                                 "MP2-DOM"),
                ("Localization", 12,  "Test Run Schedule",                                                      "IQC"),
                ("Localization", 13,  "Test Run Approval Date",                                                 "IQC"),
                ("Localization", 14,  "4M Approval Date",                                                       "IQC"),
                ("Localization", 15,  "Request Simulation to MP2",                                              "MP1-PUR"),
                ("Localization", 16,  "Simulation of Old Suppliers Stocks",                                     "MP2-OVR"),
                ("Localization", 17,  "Final PO Delivery (Date)",                                               "MP2-OVR"),
                ("Localization", 18,  "SAP Setting Change",                                                     "MP1-PUR"),
                ("Localization", 19,  "BLK and FIX Supplier",                                                  "MP1-PUR"),
                ("Localization", 20,  "Recosting Date",                                                         "MP1-PUR"),
                ("Localization", 21,  "PO issuance Date (New Supplier)",                                        "MP2-DOM"),
                ("Localization", 22,  "Parts Availability (New Supplier Delivery Date)",                        "MP2-DOM"),
                ("Localization", 23,  "Target Usage Date",                                                      "MP1-PUR"),
            };

            return rows.Select(r => new NewToolingProcessMapping
            {
                Id = id++,
                Category = r.cat,
                StepOrder = r.order,
                ProcessStep = r.step,
                Section = r.section
            }).ToArray();
        }
    }
}
