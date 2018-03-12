using System.Collections.Generic;
using System.Threading.Tasks;
using Gravity.Data;

namespace Gravity.Manager.Domain.Aws
{
    public interface IDiscoverySessionRepository : IGenericRepository<DiscoverySession, long>
    {  
        Task<List<DiscoverySession>> GetDiscoverySessionsWithAccountsAsync();

    }


   
}