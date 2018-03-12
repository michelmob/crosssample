using System.Collections.Generic;
using System.Threading.Tasks;
using Gravity.Data;
using Gravity.Manager.Domain.Dependencies;

namespace Gravity.Manager.Domain.Audits
{
    public interface IDependencyFindingRepository : IGenericRepository<DependencyFinding, long>
    {
        /// <summary>
        /// Gets all findings for specified discovery session, including navigation properties.
        /// </summary>
        Task<List<DependencyFinding>> GetSessionFindingsAsync(long discoverySessionId);
    }
}