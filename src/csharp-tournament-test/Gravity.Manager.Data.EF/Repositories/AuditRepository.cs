using System.Linq;
using System.Threading.Tasks;
using Gravity.Data;
using Gravity.Data.EF;
using Gravity.Manager.Data.Entities;
using Gravity.Manager.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravity.Manager.Data.EF.Repositories
{
    internal class AuditRepository : EntityRepository<AuditEntry>, IAuditRepository
    {
        public AuditRepository(DbContext context) : base(context)
        {
            // No-op.
        }

        public async Task<DataPage<AuditEntry>> GetPageAsync(int pageIndex, int pageSize)
        {
            var count = await AsQueryable().CountAsync();
            var items = await AsQueryable()
                .Include(x => x.User)
                .OrderByDescending(x => x.Date)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new DataPage<AuditEntry>(items, count, pageSize, pageIndex);
        }
    }
}