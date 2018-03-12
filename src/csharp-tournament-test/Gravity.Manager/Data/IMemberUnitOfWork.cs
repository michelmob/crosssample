using Gravity.Data;
using Gravity.Manager.Data.Repositories;

namespace Gravity.Manager.Data
{
    public interface IMemberUnitOfWork : IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        
        IOrganizationRepository OrganizationRepository { get; }
        
    }
}