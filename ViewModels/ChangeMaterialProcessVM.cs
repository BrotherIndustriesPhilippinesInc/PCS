namespace PartsControlSystem.ViewModels
{
    public class ChangeMaterialProcessVM
    {
        public string ControlNumber { get; set; }
        public string Section { get; set; }
        public string Model { get; set; }
        public string ChildPartcode { get; set; }
        public string PartName { get; set; }
        public string Supplier { get; set; }
        public string BIPHMoldNo { get; set; }
        public string SupplierMoldNo { get; set; }
        public string MoldMaker { get; set; }
        public string ToolingManagement { get; set; }
        public string ProcessStep { get; set; }
        public int StepOrder { get; set; }
        public string NextProcessStep { get; set; }
    }
}