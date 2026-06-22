using System.ComponentModel.DataAnnotations.Schema;

namespace PartsControlSystem.Models
{
    [Table("activity_current_process")]
    public class ActivityCurrentProcess
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("control_number")]
        public string ControlNumber { get; set; }

        [Column("current_process")]
        public string CurrentProcess { get; set; }

        [Column("update_at")]
        public DateTime UpdateAt { get; set; }

        [Column("is_delay")]
        public bool IsDelay { get; set; } = false;
    }
}