using Gravity.Data;

namespace Gravity.Manager.Domain.Audits
{
    public interface IAuditRepository : IGenericRepository<AuditEntry, long>
    {
        // No-op.
    }
}