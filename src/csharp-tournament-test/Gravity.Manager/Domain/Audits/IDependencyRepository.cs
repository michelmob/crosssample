using Gravity.Data;
using Gravity.Manager.Domain.Dependencies;

namespace Gravity.Manager.Domain.Audits
{
    public interface IDependencyRepository : IGenericRepository<Dependency, long>
    {
        // No-op.
    }
}