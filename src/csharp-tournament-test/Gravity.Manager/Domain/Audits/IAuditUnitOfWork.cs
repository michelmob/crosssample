using Gravity.Data;

namespace Gravity.Manager.Domain.Audits
{
    public interface IAuditUnitOfWork : IUnitOfWork
    {
        IAuditRepository Audit { get; }
    }
}