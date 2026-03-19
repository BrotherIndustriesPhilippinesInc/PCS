using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Models;

namespace PartsControlSystem.Data
{
    public class SqlServerAppDbContextCas : DbContext
    {
        public SqlServerAppDbContextCas(DbContextOptions<SqlServerAppDbContextCas> options) : base(options) { }
       
        public DbSet<CasSystemApproverList> Tbl_System_Approver_list { get; set; }
        public DbSet<Tbl_LOGIN_Request> Tbl_LOGIN_Request { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CasSystemApproverList>().ToTable("Tbl_System_Approver_list");
            modelBuilder.Entity<Tbl_LOGIN_Request>().ToTable("Tbl_lOGIN_Request");
        }
    }
}
