namespace PartsControlSystem.ViewModels
{
    public class BasedImportData
    {
        // From ImportData
        public int ImportDataId { get; set; }
        public string ControlNo { get; set; }
        public string Section { get; set; }
        public string ToolingType { get; set; }
        public string ToolingCategory { get; set; }
        public string Model { get; set; }
        public string PartCode { get; set; }
        public string PartName { get; set; }
        public string ToolingManagement { get; set; }
        public string Supplier { get; set; }
        public string BiphMoldNo { get; set; }
        public string SupplierMoldNo { get; set; }
        public string MoldMaker { get; set; }
        public string ChildPart { get; set; }
        public string Activity { get; set; }

        //// Common computed fields
        //public DateTime? LimitDate { get; set; }
        //public int RemainingDays { get; set; }
    }
}
