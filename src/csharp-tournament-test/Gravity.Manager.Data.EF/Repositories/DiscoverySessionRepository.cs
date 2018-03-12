using Gravity.Data.EF;
using Gravity.Manager.Data.Entities;
using Gravity.Manager.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravity.Manager.Data.EF.Repositories
{
    internal class DiscoverySessionRepository : EntityRepository<DiscoverySession>, IDiscoverySessionRepository
    {
        public DiscoverySessionRepository(DbContext context) : base(context)
        {
            // No-op.
        }
    }
}