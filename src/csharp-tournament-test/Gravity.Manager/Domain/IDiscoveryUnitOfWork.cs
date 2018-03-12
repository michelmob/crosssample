using System.Collections.Generic;
using System.Threading.Tasks;
using Gravity.Data;
using Gravity.Manager.Domain.Audits;
using Gravity.Manager.Domain.Aws;

namespace Gravity.Manager.Domain
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
        
    }
}