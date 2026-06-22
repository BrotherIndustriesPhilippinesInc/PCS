using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartsControlSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNewToolingProcessMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clear all existing rows and re-insert with updated data
            migrationBuilder.Sql("DELETE FROM new_tooling_process_mapping;");

            // ── MULTIPLE PROCUREMENT (22 steps) ──────────────────────────
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (1, 'Multiple Procurement', 1, 'Mold LOA', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (2, 'Multiple Procurement', 2, 'Material LOA', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (3, 'Multiple Procurement', 3, 'Manual FC to new supplier', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (4, 'Multiple Procurement', 4, 'Tooling PO issuance', 'MP2-TOOLING', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (5, 'Multiple Procurement', 5, 'Tooling Transfer Date', 'MP2-TOOLING', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (6, 'Multiple Procurement', 6, '4M Application Date', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (7, 'Multiple Procurement', 7, 'Kataken PH Sample Submission', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (8, 'Multiple Procurement', 8, 'Kataken PH Sample Approval', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (9, 'Multiple Procurement', 9, 'Availability of Parts Packaging Standard', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (10, 'Multiple Procurement', 10, 'Open sourcelist (New Supplier) / Updated Price input', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (11, 'Multiple Procurement', 11, 'Procurement Type Change', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (12, 'Multiple Procurement', 12, 'Test Run PO Request Date', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (13, 'Multiple Procurement', 13, 'Return of Special procurement type', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (14, 'Multiple Procurement', 14, 'Test Run PO Date', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (15, 'Multiple Procurement', 15, 'Test Run Delivery Date', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (16, 'Multiple Procurement', 16, 'Test Run Schedule', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (17, 'Multiple Procurement', 17, 'Test Run Approval Date', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (18, 'Multiple Procurement', 18, 'Confirmation of Parts Availability', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (19, 'Multiple Procurement', 19, 'Quota Arrangement SAP input', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (20, 'Multiple Procurement', 20, 'PO issuance Date (New Supplier)', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (21, 'Multiple Procurement', 21, 'Parts Delivery Date (New Supplier Delivery Date)', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (22, 'Multiple Procurement', 22, 'Target Usage Date', 'MP1/MP2', 0);");

            // ── SUPPLIER CHANGE (29 steps) ────────────────────────────────
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (23, 'Supplier Change', 1, 'Mold LOA', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (24, 'Supplier Change', 2, 'Material LOA', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (25, 'Supplier Change', 3, 'Manual FC to new supplier', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (26, 'Supplier Change', 4, 'Tooling PO issuance', 'MP2-TOOLING', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (27, 'Supplier Change', 5, 'Tooling Transfer Date', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (28, 'Supplier Change', 6, '4M Application Date', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (29, 'Supplier Change', 7, 'Kataken PH Sample Submission', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (30, 'Supplier Change', 8, 'Kataken PH Sample Approval', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (31, 'Supplier Change', 9, 'DE Sample Received Date', 'DE', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (32, 'Supplier Change', 10, 'DE Sample Approval', 'DE', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (33, 'Supplier Change', 11, 'QA Sample Received Date', 'QA', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (34, 'Supplier Change', 12, 'QA Sample Approval', 'QA', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (35, 'Supplier Change', 13, 'Availability of Parts Packaging Standard', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (36, 'Supplier Change', 14, 'Open sourcelist (New Supplier)', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (37, 'Supplier Change', 15, 'Test Run PO Request Date', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (38, 'Supplier Change', 16, 'Test Run PO Date', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (39, 'Supplier Change', 17, 'Test Run Delivery Date', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (40, 'Supplier Change', 18, 'Test Run Schedule', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (41, 'Supplier Change', 19, 'Test Run Approval Date', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (42, 'Supplier Change', 20, '4M Approval Date', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (43, 'Supplier Change', 21, 'Request Simulation to MP2', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (44, 'Supplier Change', 22, 'Simulation of Old Suppliers Stocks', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (45, 'Supplier Change', 23, 'SAP Setting Change', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (46, 'Supplier Change', 24, 'Final PO Delivery (Date)', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (47, 'Supplier Change', 25, 'BLK and FIX Supplier', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (48, 'Supplier Change', 26, 'Recosting Date', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (49, 'Supplier Change', 27, 'PO issuance Date (New Supplier)', 'PDC-Loc', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (50, 'Supplier Change', 28, 'Parts Availability (New Supplier Delivery Date)', 'PDC-Loc', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (51, 'Supplier Change', 29, 'Target Usage Date', 'MP2', 0);");

            // ── LOCALIZATION (24 steps) ───────────────────────────────────
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (52, 'Localization', 1, 'Input Date of MP1 for parts with localization', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (53, 'Localization', 2, 'Tooling PO Issued Date', 'MP2-TOOL', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (54, 'Localization', 3, 'Drawing Issuance to Supplier', 'PC-DCI', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (55, 'Localization', 4, 'Tooling Transfer Date', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (56, 'Localization', 5, '4M Application Date', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (57, 'Localization', 6, 'Kataken PH Sample Submission', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (58, 'Localization', 7, 'Kataken PH Sample Approval', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (59, 'Localization', 8, 'Procurement Type Change', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (60, 'Localization', 9, 'Open Sourcelist Local', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (61, 'Localization', 10, 'Test Run PO Request Date', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (62, 'Localization', 11, 'Test Run PO Date', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (63, 'Localization', 12, 'Test Run Delivery Date', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (64, 'Localization', 13, 'Test Run Schedule', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (65, 'Localization', 14, 'Test Run Approval Date', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (66, 'Localization', 15, '4M Approval Date', 'IQC', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (67, 'Localization', 16, 'Request Simulation to MP2', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (68, 'Localization', 17, 'Simulation of Old Suppliers Stocks', 'PDC-Loc', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (69, 'Localization', 18, 'SAP Setting Change', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (70, 'Localization', 19, 'Final PO Delivery (Date)', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (71, 'Localization', 20, 'BLK and FIX Supplier', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (72, 'Localization', 21, 'Recosting Date', 'MP1-PUR', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (73, 'Localization', 22, 'PO issuance Date (New Supplier)', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (74, 'Localization', 23, 'Parts Availability (New Supplier Delivery Date)', 'MP2-DOM', 0);");
            migrationBuilder.Sql("INSERT INTO new_tooling_process_mapping (\"Id\", \"Category\", \"StepOrder\", \"ProcessStep\", \"Section\", \"LeadTimeDays\") VALUES (75, 'Localization', 24, 'Target Usage Date', 'MP1-PUR', 0);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM new_tooling_process_mapping;");
        }
    }
}