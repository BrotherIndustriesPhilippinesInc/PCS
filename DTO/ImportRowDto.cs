namespace PartsControlSystem.DTO
{
    public class ImportRowDto
    {
        public string ControlNo { get; set; }
        public string Section { get; set; }

        public string MotherMoldCode { get; set; }
        public string ChildPartcode { get; set; }
        public string PartName { get; set; }
        public string Model { get; set; }
        public string Supplier { get; set; }
        public string MoldMaker { get; set; }

        public string SupplierMoldNo { get; set; }
        public string BiphMoldNo { get; set; }
        public string ToolingManagement { get; set; }

        public string Activity { get; set; }
        public string ReasonOfChange { get; set; }

        // dynamic columns
        public Dictionary<string, string> DynamicValues { get; set; }
            = new Dictionary<string, string>();
    }
}
