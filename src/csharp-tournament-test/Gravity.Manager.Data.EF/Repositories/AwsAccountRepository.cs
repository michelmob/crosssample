using Gravity.Data.EF;
using Gravity.Manager.Domain.Aws;
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