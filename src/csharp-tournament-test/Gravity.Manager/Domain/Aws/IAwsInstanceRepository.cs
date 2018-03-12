using Gravity.Data;

namespace Gravity.Manager.Domain.Aws
{
    public interface IAwsInstanceRepository : IGenericRepository<AwsInstance, long>
    {
        // No-op.
    }
}