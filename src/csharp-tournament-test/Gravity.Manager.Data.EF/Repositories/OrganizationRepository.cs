using Gravity.Data.EF;
using Gravity.Manager.Domain.Organizations;
using Microsoft.EntityFrameworkCore;

namespace Gravity.Manager.Data.EF.Repositories
{
    public class OrganizationRepository : EntityRepository<Organization>, IOrganizationRepository
    {
        public OrganizationRepository(DbContext context) : base(context)
        {
            // No-op.
        }
    }
}