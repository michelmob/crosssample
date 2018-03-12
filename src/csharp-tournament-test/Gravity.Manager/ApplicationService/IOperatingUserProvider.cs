using Gravity.Manager.Domain.Organizations;

namespace Gravity.Manager.ApplicationService
{
    public interface IOperatingUserProvider
    {
        User GetOperatingUser();
    }
}