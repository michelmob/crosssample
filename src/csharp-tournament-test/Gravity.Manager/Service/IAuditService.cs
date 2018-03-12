using System;
using System.Threading.Tasks;
using Gravity.Data;
using Gravity.Manager.Data;
using Gravity.Manager.Data.Entities;

namespace Gravity.Manager.Service
{
    public interface IAuditService : IDisposable
    {
        Task<DataPage<AuditEntry>> GetAuditPageAsync(int pageIndex, int pageSize);
    }
}