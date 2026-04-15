using Microsoft.EntityFrameworkCore;
using PartsControlSystem.Models;

namespace PartsControlSystem.Data
{
    public class PostgreAppDbContext : DbContext
    {
        public PostgreAppDbContext(DbContextOptions<PostgreAppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<ImportData> ImportDatas { get; set; }
        public DbSet<_4mForm> _4mForms { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<LeadTime> LeadTimes { get; set; }
        public DbSet<UpdateActivityMatrix> UpdateActivityMatrices { get; set; }
        public DbSet<UpdateActivityData> UpdateActivityData { get; set; }
        public DbSet<ActivityCurrentProcess> ActivityCurrentProcesses { get; set; }
        public DbSet<MP2_ToolingQuotationRequestApproval> ToolingQuotationRequestApproval { get; set; }
        public DbSet<MP2_ToolingRequestOrder> MP2ToolingRequestOrder { get; set; }
        public DbSet<MP2_ToolingPoIssuance> MP2ToolingPoIssuance { get; set; }
        public DbSet<SQC_DFMQCDApproval> SQCDFMQCDApprovals { get; set; }
        public DbSet<MP2_ToolingTransfer> MP2ToolingTransfers { get; set; }
        public DbSet<MP2_ToolingFabrication> MP2ToolingFabrications { get; set; }
        public DbSet<IQC_KatakenSubmission> IQCKatakenSubmissions { get; set; }
        public DbSet<IQC_KatakenFinish> IQCKatakenFinish { get; set; }
        public DbSet<DE_Evaluation> DEEvaluation { get; set; }
        public DbSet<QA_SpecialEvaluation> QASpecialEvaluations { get; set; }
        public DbSet<IQC_TestRun> IQCTestRuns { get; set; }

        public DbSet<ViewActivityMonitoring> ViewActivityMonitoring { get; set; }
        public DbSet<MP2_Capa_PDC> MP2_Capa_PDCs { get; set; }  
        public DbSet<TransactionLogs> TransactionLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ViewActivityMonitoring>(entity =>
            {
                entity.HasNoKey(); // because it is a VIEW
                entity.ToView("view_activity_monitoring", "public");
            });


        }
    } 


}
