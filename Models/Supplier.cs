using System.ComponentModel.DataAnnotations;

namespace PartsControlSystem.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Supplier name is required")]
        public string SupplierName { get; set; }

        [Required(ErrorMessage = "Part category is required")]
        public string PartsCategory { get; set; }

        [Required(ErrorMessage = "Supplier type is required")]
        public string Location { get; set; } // Local or Oversea

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^([^\s@]+@[^\s@]+\.[^\s@]+)(;\s*[^\s@]+@[^\s@]+\.[^\s@]+)*$",
            ErrorMessage = "Emails must be separated by semicolon and valid format")]
        public string Email { get; set; } // Multiple emails separated by semicolon
    }

}
