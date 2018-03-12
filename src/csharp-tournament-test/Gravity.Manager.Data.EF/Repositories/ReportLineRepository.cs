using Gravity.Data.EF;
using Gravity.Manager.Data.Entities;
using Gravity.Manager.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravity.Manager.Data.EF.Repositories
{
    public class ReportLineRepository : EntityRepository<ReportLine>, IReportLineRepository
    {
        public ReportLineRepository(DbContext context) : base(context)
        {
            // No-op.
        }
    }
}