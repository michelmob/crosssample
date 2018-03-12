using Gravity.Data;
using Gravity.Manager.Data.Repositories;

namespace Gravity.Manager.Data
{
    public interface IAuditUnitOfWork : IUnitOfWork
    {
        IAuditRepository Audit { get; }
    }
}