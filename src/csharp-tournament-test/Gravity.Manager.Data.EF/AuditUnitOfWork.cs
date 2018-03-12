using Gravity.Manager.Data.EF.Repositories;
using Gravity.Manager.Data.Repositories;

namespace Gravity.Manager.Data.EF
{
    public class AuditUnitOfWork : GravityManagerUnitOfWorkBase, IAuditUnitOfWork
    {
        public AuditUnitOfWork(GravityManagerDbContext context) : base(context)
        {
            Audit = new AuditRepository(context);
        }

        public IAuditRepository Audit { get; }
    }
}