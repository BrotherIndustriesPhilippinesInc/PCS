using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Models;

namespace PartsControlSystem.Data
{
    public class SqlServerAppDbContext : DbContext
    {
        public SqlServerAppDbContext(DbContextOptions<SqlServerAppDbContext> options) : base(options) { }

        public DbSet<EmployeeList> T_Employee_List { get; set; }
  
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeList>().HasNoKey().ToView("T_Employee_List");
        }
    }
}
