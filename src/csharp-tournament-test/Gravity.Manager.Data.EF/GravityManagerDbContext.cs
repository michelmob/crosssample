using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Gravity.Manager.ApplicationService;
using Gravity.Manager.Domain.Audits;
using Gravity.Manager.Domain.Aws;
using Gravity.Manager.Domain.Dependencies;
using Gravity.Manager.Domain.Organizations;
using Gravity.Runtime.Serialization;
using Gravity.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Gravity.Manager.Data.EF
{
    public sealed class GravityManagerDbContext: DbContext
    {
        private readonly IOperatingUserProvider _operatingUserProvider;
        private readonly ISerializer _auditEntrySerializer;
        private readonly IDateTimeProvider _dateTimeProvider;

        // ReSharper disable once SuggestBaseTypeForParameter
        public GravityManagerDbContext(
            DbContextOptions<GravityManagerDbContext> options,
            IOperatingUserProvider userProvider, 
            ISerializer auditEntrySerializer,
            IDateTimeProvider dateTimeProvider) 
            : base(options)
        {
            _operatingUserProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));
            _auditEntrySerializer = auditEntrySerializer ?? throw new ArgumentNullException(nameof(auditEntrySerializer));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AwsAccount>().HasAlternateKey(a => a.Name);

            builder.Entity<User>().HasAlternateKey(e => e.UserName);
            
            // Make table names singular.
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName();
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            RecordAudit();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            RecordAudit();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void RecordAudit()
        {
            var entries = ChangeTracker.Entries().ToArray();

            if (entries.Length == 1 && entries[0].State == EntityState.Added && entries[0].Entity is User)
            {
                // Skip audit for user creation, because audit requires the user.
                return;
            }
            
            var operatingUser = _operatingUserProvider.GetOperatingUser();
            
            // Skip audit for API endpoints.
            // TODO: Enable this back when we have token auth.
            if (operatingUser == null)
            {
                return;
            }

            var dateTime = _dateTimeProvider.Now();

            foreach (var entry in entries)
            {
                AuditEntry audit = null;

                switch (entry.State)
                {
                    case EntityState.Deleted:
                        // Save all values as old
                        audit = new AuditEntry { OldValue = Serialize(entry.CurrentValues) };
                        break;
                    
                    case EntityState.Modified:
                        // Save changed properties
                        audit = new AuditEntry
                        {
                            NewValue = Serialize(entry.CurrentValues),
                            OldValue = Serialize(entry.OriginalValues)
                        };
                        break;
                    
                    case EntityState.Added:
                        // Save new values
                        audit = new AuditEntry { NewValue = Serialize(entry.CurrentValues) };
                        break;
                    
                    default:
                        continue;
                }
                
                // Do not create audit for audit. This should not happen.
               // Debug.Assert(!(entry.Entity is AuditEntry));

                if (!(entry.Entity is AuditEntry))
                {
                    audit.EntityName = entry.Metadata.DisplayName();
                    audit.UserId = (operatingUser).Id;
                    audit.Date = dateTime;
                    //NOTE: Was not recording audit entries
                    AuditEntries.Add(audit);
                }
            }
        }

        private string Serialize(PropertyValues values)
        {
            var sb = new StringBuilder();

            foreach (var prop in values.Properties)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }

                var val = values[prop];
                var serializedVal = (string) _auditEntrySerializer.Serialize(val);

                sb.AppendFormat("\"{0}\":{1}", prop.Name, serializedVal);
            }
            
            return sb.ToString();
        }

        public DbSet<AwsAccount> AwsAccounts { get; set; } 
        public DbSet<DiscoverySession> DiscoverySessions { get; set; } 
        public DbSet<AwsInstance> AwsInstances { get; set; } 
        public DbSet<Dependency> AwsInstanceDependencies { get; set; } 
        public DbSet<DependencyFinding> DependencyFindings { get; set; } 
        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<AuditEntry> AuditEntries { get; set; }
    }
}