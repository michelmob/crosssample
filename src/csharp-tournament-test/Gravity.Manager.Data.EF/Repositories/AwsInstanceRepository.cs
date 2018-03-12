using Gravity.Data.EF;
using Gravity.Manager.Data.Entities;
using Gravity.Manager.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravity.Manager.Data.EF.Repositories
{
    internal class AwsInstanceRepository : EntityRepository<AwsInstance>, IAwsInstanceRepository
    {
        public AwsInstanceRepository(DbContext context) : base(context)
        {
            // No-op.
        }
    }
}