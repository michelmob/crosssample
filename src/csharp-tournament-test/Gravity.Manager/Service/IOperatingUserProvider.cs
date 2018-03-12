using Gravity.Manager.Data.Entities;

namespace Gravity.Manager.Service
{
    public interface IOperatingUserProvider
    {
        User GetOperatingUser();
    }
}