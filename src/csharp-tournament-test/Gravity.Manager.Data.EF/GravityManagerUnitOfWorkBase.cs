using System.Data;
using System.Threading.Tasks;
using Gravity.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gravity.Manager.Data.EF
{
    public class GravityManagerUnitOfWorkBase : UnitOfWorkBase<GravityManagerDbContext>
    {
        protected GravityManagerUnitOfWorkBase(GravityManagerDbContext context) : base(context)
        {
            // No-op.
        }

        public override int Commit()
        {
            return Context.SaveChanges();
        }

        public override Task<int> CommitAsync()
        {
            return Context.SaveChangesAsync();
        }

        public override IDbTransaction BeginDbTransaction()
        {
            return Context.Database.BeginTransaction().GetDbTransaction();
        }
    }
}