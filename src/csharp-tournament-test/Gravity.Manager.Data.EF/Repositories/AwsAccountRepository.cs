using Gravity.Data.EF;
using Gravity.Manager.Data.Entities;
using Gravity.Manager.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravity.Manager.Data.EF.Repositories
{
    internal class AwsAccountRepository : EntityRepository<AwsAccount>, IAwsAccountRepository
    {
        public AwsAccountRepository(DbContext context) : base(context)
        {
            // No-op.
        }
    }
}