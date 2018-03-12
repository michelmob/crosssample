using System;
using System.Threading.Tasks;
using Gravity.Data;
using Gravity.Manager.Data;
using Gravity.Manager.Data.Entities;

// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleAnonymousFunction
namespace Gravity.Manager.Service
{
    public class AuditService : IAuditService
    {
        private readonly IAuditUnitOfWork _context;

        public AuditService(IAuditUnitOfWork context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public Task<DataPage<AuditEntry>> GetAuditPageAsync(int pageIndex, int pageSize)
        {
            return _context.Audit.GetPageAsync(pageIndex, pageSize,
                include: a => a.User, orderBy: a => a.Date, descending: true);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}