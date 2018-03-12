using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gravity.Data.EF;
using Gravity.Manager.Data.Entities;
using Gravity.Manager.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravity.Manager.Data.EF.Repositories
{
    internal class DependencyFindingRepository : EntityRepository<DependencyFinding>, IDependencyFindingRepository
    {
        public DependencyFindingRepository(DbContext context) : base(context)
        {
            // No-op.
        }

        public Task<List<DependencyFinding>> GetSessionFindingsAsync(long discoverySessionId)
        {
            // "Include" translates to INNER JOIN, so this is a single query.
            return AsQueryable()
                .Where(x => x.Dependency.SourceAwsInstance.DiscoverySessionId == discoverySessionId)
                .Include(x => x.Dependency.SourceAwsInstance.DiscoverySession.AwsAccount)
                .Include(x => x.Dependency.TargetAwsInstance.DiscoverySession.AwsAccount)
                .Include(x => x.Dependency.SourceAwsInstance.ReportLines)
                .Include(x => x.Dependency.TargetAwsInstance.ReportLines)
                .ToListAsync();
        }
    }
}