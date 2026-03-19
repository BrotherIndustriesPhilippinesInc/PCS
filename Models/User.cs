using System.ComponentModel.DataAnnotations.Schema;

namespace PartsControlSystem.Models
{
    public class User
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("employee_id")]
        public string EmployeeId { get; set; }
        [Column("adid")]
        public string ADID { get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("email")]
        public string Email { get; set; }
        [Column("section")]
        public string Section { get; set; }
        [Column("department")]
        public string Department { get; set; }
        [Column("position")]
        public string position { get; set; }
        [Column("approver_role")]
        public string ApproverRole { get; set; }
        [Column("authority")]
        public string Authority { get; set; }
        [Column("created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
