using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gravity.Manager.Data.EF.Repositories;
using Gravity.Manager.Data.Entities;
using Gravity.Manager.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Gravity.Manager.Data.EF
{
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

        public async Task<AwsAccount> GetOrCreateAwsAccountAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }

            using (var tx = Context.Database.BeginTransaction())
            {
                if (await Context.AwsAccounts.AsQueryable().SingleOrDefaultAsync(x => x.Name == name) is AwsAccount acc)
                {
                    return acc;
                }

                var newAcc = new AwsAccount{Name = name};
                
                await AwsAccounts.InsertAsync(newAcc);

                return newAcc;
            }
        }

        public Task<List<DiscoverySession>> GetDiscoverySessionsWithAccountsAsync()
        {
            return Context.DiscoverySessions.Include(x => x.AwsAccount).ToListAsync();
        }
    }
}