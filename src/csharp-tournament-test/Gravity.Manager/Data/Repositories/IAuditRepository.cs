using System.Threading.Tasks;
using Gravity.Data;
using Gravity.Manager.Data.Entities;

namespace Gravity.Manager.Data.Repositories
{
    public interface IAuditRepository : IGenericRepository<AuditEntry, long>
    {
        // No-op.
    }
}