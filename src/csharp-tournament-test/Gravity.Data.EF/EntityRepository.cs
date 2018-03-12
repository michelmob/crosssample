using Microsoft.EntityFrameworkCore;

namespace Gravity.Data.EF
{
    public class EntityRepository<TEntity> : GenericRepository<TEntity, long> where TEntity : EntityBase
    {
        protected EntityRepository(DbContext context) : base(context)
        {
            // No-op.
        }
    }
}