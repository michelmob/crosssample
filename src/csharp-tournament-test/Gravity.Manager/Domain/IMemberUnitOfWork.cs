using Gravity.Data;
using Gravity.Manager.Domain.Organizations;

namespace Gravity.Manager.Domain
{
    public interface IMemberUnitOfWork : IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        
        IOrganizationRepository OrganizationRepository { get; }
        
    }
}