namespace PartsControlSystem.DTO
{
    /// <summary>
    /// Updated DTO — now includes ActivityName so the controller
    /// can derive the correct category without a separate dropdown.
    /// </summary>
    public class SaveNewToolingLocalizationProcessDto
    {
        public string ControlNumber { get; set; }

        /// <summary>
        /// The full activity name from the card:
        /// "New Tooling / Localization" | "Supplier Change / Localization" | "Multiple Procurement / Localization"
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// Derived category — kept for fallback compatibility.
        /// Prefer ActivityName for routing.
        /// </summary>
        public string? Category { get; set; }

        public string ProcessStep { get; set; }
        public int StepOrder { get; set; }

        public DateTime? TargetDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string? Result { get; set; }
        public string? ReferenceNo { get; set; }
        public string? Remarks { get; set; }
        public string? FinalPOIssuedDate { get; set; }
    }
}