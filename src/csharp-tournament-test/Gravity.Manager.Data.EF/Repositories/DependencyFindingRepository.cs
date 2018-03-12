using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gravity.Data.EF;
using Gravity.Manager.Domain.Audits;
using Gravity.Manager.Domain.Dependencies;
using Microsoft.EntityFrameworkCore;

namespace Gravity.Manager.Data.EF.Repositories
{
    internal class DependencyFindingRepository : EntityRepository<DependencyFinding>, IDependencyFindingRepository
    {
        public DependencyFindingRepository(DbContext context) : base(context)
        {
            // No-op.
        }

        public async Task<List<DependencyFinding>> GetSessionFindingsAsync(long discoverySessionId)
        {
            // "Include" translates to INNER JOIN, so this is a single query.
            //No needs to include if this is a simple query, you are loading unecessary entities
            //
            
            var bdSet = base.AsQueryable();

            var query = from df in bdSet
                        where df.Dependency!=null && df.Dependency.SourceAwsInstance!=null &&
                        df.Dependency.SourceAwsInstance.DiscoverySessionId == discoverySessionId
                        select df;

            return await query.ToListAsync();

            //return AsQueryable()
            //    .Where(x => x.Dependency.SourceAwsInstance.DiscoverySessionId == discoverySessionId)
            //    //.Include(x => x.Dependency.SourceAwsInstance.DiscoverySession.AwsAccount)
            //    //.Include(x => x.Dependency.TargetAwsInstance.DiscoverySession.AwsAccount)
            //    //.Include(x => x.Dependency.SourceAwsInstance.ReportLines)
            //    //.Include(x => x.Dependency.TargetAwsInstance.ReportLines)
            //    .ToListAsync();
        }
    }
}