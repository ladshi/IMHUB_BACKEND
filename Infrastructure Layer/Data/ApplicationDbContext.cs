using IMHub.Domain.Common;
using IMHub.Domain.Entities;
using IMHub.Domain.Entities.Support;
using IMHub.Domain.Entities.Workflow;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IMHub.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ==========================================
        // 1. IDENTITY & TENANCY (5 Tables)
        // ==========================================
        public DbSet<PlatformAdmin> PlatformAdmins { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        // Note: UserProfile is accessed via User.Profile usually, but good to have a Set if needed
        public DbSet<UserProfile> UserProfiles { get; set; }

        // ==========================================
        // 2. TEMPLATE SYSTEM (4 Tables)
        // ==========================================
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateVersion> TemplateVersions { get; set; }
        public DbSet<TemplatePage> TemplatePages { get; set; }
        public DbSet<TemplateField> TemplateFields { get; set; }

        // ==========================================
        // 3. EXECUTION & CONTENT (3 Tables)
        // ==========================================
        public DbSet<CsvUpload> CsvUploads { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<ContentFieldValue> ContentFieldValues { get; set; }

        // ==========================================
        // 4. PRINT & DISTRIBUTION (3 Tables)
        // ==========================================
        public DbSet<Printer> Printers { get; set; }
        public DbSet<Sendout> Sendouts { get; set; }
        public DbSet<SendoutStatusHistory> SendoutStatusHistories { get; set; }
        public DbSet<Distribution> Distributions { get; set; }

        // ==========================================
        // 5. WORKFLOW, ASSETS & LOGS (7 Tables)
        // ==========================================
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<FileStorage> FileStorages { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }
        public DbSet<LookupValue> LookupValues { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // -----------------------------------------------------------------------
            // 1. AUTOMATIC CONFIGURATION LOADING (The Senior Dev Trick)
            // -----------------------------------------------------------------------
            // This line looks into your 'EntityConfiguration' folder (and anywhere in this project)
            // and applies any class that inherits from IEntityTypeConfiguration<T>.
            // This keeps this file clean!
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // -----------------------------------------------------------------------
            // 2. GLOBAL QUERY FILTERS (Soft Delete)
            // -----------------------------------------------------------------------
            // Automatically applies "WHERE IsDeleted = 0" to every query
            var method = typeof(ApplicationDbContext)
               .GetMethod(nameof(ConfigureGlobalFilters), BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // Reflection :separate filters for each tables
                    method?.MakeGenericMethod(entityType.ClrType)
                          .Invoke(null, new object[] { builder });
                }
            }

            // -----------------------------------------------------------------------
            // 3. GLOBAL DELETE BEHAVIOR (Safety)
            // -----------------------------------------------------------------------
            // Prevents accidentally deleting a parent record if children exist (Restrict Cascade)
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        private static void ConfigureGlobalFilters<T>(ModelBuilder builder) where T : BaseEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
        }
        

        // Helper Method for the Global Query Filter
        private static System.Linq.Expressions.Expression<Func<T, bool>> ConvertFilterExpression<T>(
            System.Linq.Expressions.Expression<Func<T, bool>> filterExpression)
        {
            return filterExpression;
        }
    }
}