using System.Collections.Generic;
using System.Threading.Tasks;
using Gravity.Data.EF;
using Gravity.Manager.Domain.Aws;
using Microsoft.EntityFrameworkCore;

namespace Gravity.Manager.Data.EF.Repositories
{
    internal class DiscoverySessionRepository : EntityRepository<DiscoverySession>, IDiscoverySessionRepository
    {
        public DiscoverySessionRepository(DbContext context) : base(context)
        {
            // No-op.
        }

        public Task<List<DiscoverySession>> GetDiscoverySessionsWithAccountsAsync()
        {   
            return AsQueryable().Include(x => x.AwsAccount).ToListAsync();
        }
    }
}