using Gravity.Data.EF;
using Gravity.Manager.Domain.Audits;
using Gravity.Manager.Domain.Dependencies;
using Microsoft.EntityFrameworkCore;

namespace Gravity.Manager.Data.EF.Repositories
{
    internal class DependencyRepository : EntityRepository<Dependency>, IDependencyRepository
    {
        public DependencyRepository(DbContext context) : base(context)
        {
            // No-op.
        }
    }
}