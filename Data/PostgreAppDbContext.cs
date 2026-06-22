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


        public DbSet<NewToolingCategory> NewToolingCategories { get; set; }
        public DbSet<NewToolingLocalizationProcess> NewToolingLocalizationProcesses { get; set; }
        public DbSet<NewToolingProcessMapping> NewToolingProcessMappings { get; set; }

        public DbSet<ViewLocalizationMonitoring> ViewLocalizationMonitoring { get; set; }
        public DbSet<ViewSupplierChangeMonitoring> ViewSupplierChangeMonitoring { get; set; }
        public DbSet<ViewMultipleProcurementMonitoring> ViewMultipleProcurementMonitoring { get; set; }

        public DbSet<ChangeMaterialProcess> ChangeMaterialProcesses { get; set; }
        public DbSet<ChangeMaterialProcessMapping> ChangeMaterialProcessMappings { get; set; }
        public DbSet<ViewChangeMaterialMonitoring> ViewChangeMaterialMonitoring { get; set; }


        public DbSet<Other4MProcessMapping> Other4MProcessMappings { get; set; }
        public DbSet<Other4MProcess> Other4MProcesses { get; set; }
 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ViewActivityMonitoring>(entity =>
            {
                entity.HasNoKey(); // because it is a VIEW
                entity.ToView("view_activity_monitoring", "public");
            });

            // Unique constraint: one category assignment per ControlNumber
            modelBuilder.Entity<NewToolingCategory>()
                .HasIndex(x => x.ControlNumber)
                .IsUnique();

            // Unique constraint: one record per ControlNumber + Category + ProcessStep
            modelBuilder.Entity<NewToolingLocalizationProcess>()
                .HasIndex(x => new { x.ControlNumber, x.Category, x.ProcessStep })
                .IsUnique();

            // Seed the process mapping
            modelBuilder.Entity<NewToolingProcessMapping>()
                .HasData(NewToolingProcessMappingSeed.GetSeedData());

            modelBuilder.Entity<ViewLocalizationMonitoring>(e =>
            {
                e.HasNoKey();
                e.ToView("ViewLocalizationMonitoring");
            });

            modelBuilder.Entity<ViewSupplierChangeMonitoring>(e =>
            {
                e.HasNoKey();
                e.ToView("ViewSupplierChangeMonitoring");
            });

            modelBuilder.Entity<ViewMultipleProcurementMonitoring>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("view_multiple_procurement_monitoring", "public");
            });

            modelBuilder.Entity<ChangeMaterialProcess>()
                .HasIndex(x => new { x.ControlNumber, x.ProcessStep })
                .IsUnique();

            modelBuilder.Entity<ChangeMaterialProcessMapping>()
                .HasData(ChangeMaterialProcessMappingSeed.GetSeedData());

            modelBuilder.Entity<ViewChangeMaterialMonitoring>(e =>
            {
                e.HasNoKey();
                e.ToView("view_change_material_monitoring", "public");
            });

            modelBuilder.Entity<Other4MProcess>()
                .HasIndex(x => x.ControlNumber)
                .IsUnique();

            modelBuilder.Entity<Other4MProcessMapping>()
                .HasData(Other4MProcessMappingSeed.GetSeedData());

        }

    } 


}
