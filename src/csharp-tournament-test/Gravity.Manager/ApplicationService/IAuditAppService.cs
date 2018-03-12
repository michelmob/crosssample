using System;
using System.Threading.Tasks;
using Gravity.Data;
using Gravity.Manager.Domain.Audits;

namespace Gravity.Manager.ApplicationService
{
    public interface IAuditAppService : IDisposable
    {
        Task<DataPage<AuditEntry>> GetAuditPageAsync(int pageIndex, int pageSize);
    }
}