
using Gravity.Manager.Data.EF.Repositories;
using Gravity.Manager.Domain;
using Gravity.Manager.Domain.Audits;
using Gravity.Manager.Domain.Aws;

namespace Gravity.Manager.Data.EF
{
    /// <summary>
    /// Refactory
    /// </summary>
    public sealed class DiscoveryUnitOfWork : GravityManagerUnitOfWorkBase, IDiscoveryUnitOfWork
    {
        public DiscoveryUnitOfWork(GravityManagerDbContext dbContext) : base(dbContext)
        {
            AwsAccounts = new AwsAccountRepository(Context);
            AwsInstances = new AwsInstanceRepository(Context);
            DiscoverySessions = new DiscoverySessionRepository(Context);
            Dependencies = new DependencyRepository(Context);
            DependencyFindings = new DependencyFindingRepository(Context);
            ReportLines = new ReportLineRepository(Context);
        }

        public IAwsAccountRepository AwsAccounts { get; }
        public IAwsInstanceRepository AwsInstances { get; }
        public IDiscoverySessionRepository DiscoverySessions { get; }
        public IDependencyRepository Dependencies { get; }
        public IDependencyFindingRepository DependencyFindings { get; }
        public IReportLineRepository ReportLines { get; }


        ////MIxing Unit of work and 
        //public async Task<AwsAccount> GetOrCreateAwsAccountAsync(string name)
        //{
        //    if (string.IsNullOrWhiteSpace(name))
        //    {
        //        throw new ArgumentException(nameof(name));
        //    }

        //    using (var tx = Context.Database.BeginTransaction())
        //    {
        //        if (await Context.AwsAccounts.AsQueryable().SingleOrDefaultAsync(x => x.Name == name) is AwsAccount acc)
        //        {
        //            return acc;
        //        }

        //        var newAcc = new AwsAccount{Name = name};
                
        //        await AwsAccounts.InsertAsync(newAcc);
                
        //        return newAcc;
        //    }
        //}

    }
}