using Gravity.Data;

namespace Gravity.Manager.Domain.Aws
{
    public interface IAwsAccountRepository : IGenericRepository<AwsAccount, long>
    {
        // No-op.
    }
}