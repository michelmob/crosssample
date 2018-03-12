using System;
using Gravity.Manager.Domain;
using Gravity.Manager.Domain.Organizations;

namespace Gravity.Manager.Data.EF
{
    public class MemberUnitOfWork : GravityManagerUnitOfWorkBase, IMemberUnitOfWork
    {
        public MemberUnitOfWork(GravityManagerDbContext context
            , IUserRepository userRepository
            , IOrganizationRepository organizationRepository
        ) : base(context)
        {
            UserRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            OrganizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
        }

        public IUserRepository UserRepository { get; }
        public IOrganizationRepository OrganizationRepository { get; }
    }
}