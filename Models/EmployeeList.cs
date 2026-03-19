using System.ComponentModel.DataAnnotations.Schema;

namespace PartsControlSystem.Models
{
    public class EmployeeList
    {
        [Column("EmpNo")]
        public string EmployeeId { get; set; }

        public string ADID { get; set; }

        [Column("First_Name")]
        public string FirstName { get; set; }
        [Column("Last_Name")]
        public string LastName { get; set; }
        [Column("Email")]
        public string Email { get; set; }
        [Column("Section")]
        public string Section { get; set; }
        [Column("Department")]
        public string Department { get; set; }
        public string Position { get; set; }
    }
}
