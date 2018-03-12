using System.Collections.Generic;
using System.Threading.Tasks;
using Gravity.Data;
using Gravity.Manager.Data.Entities;
using Gravity.Manager.Data.Repositories;

namespace Gravity.Manager.Data
{
    /// <summary>
    /// Dependency Matrix unit of work (context).
    /// </summary>
    public interface IDiscoveryUnitOfWork : IUnitOfWork
    {
        IAwsAccountRepository AwsAccounts { get; }
        IAwsInstanceRepository AwsInstances { get; }
        IDiscoverySessionRepository DiscoverySessions { get; }
        IDependencyRepository Dependencies { get; }
        IDependencyFindingRepository DependencyFindings { get; }
        IReportLineRepository ReportLines { get; }

        Task<AwsAccount> GetOrCreateAwsAccountAsync(string name);
        Task<List<DiscoverySession>> GetDiscoverySessionsWithAccountsAsync();
    }
}