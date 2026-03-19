namespace PartsControlSystem.DTO
{
    public class ImportSaveRequest
    {
        public string Section { get; set; }
        public string Activity { get; set; }

        public List<ImportRowDto> Rows { get; set; }
    }
}
