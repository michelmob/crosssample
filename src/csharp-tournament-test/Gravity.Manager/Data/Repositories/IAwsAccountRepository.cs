using Gravity.Data;
using Gravity.Manager.Data.Entities;

namespace Gravity.Manager.Data.Repositories
{
    public interface IAwsAccountRepository : IGenericRepository<AwsAccount, long>
    {
        // No-op.
    }
}