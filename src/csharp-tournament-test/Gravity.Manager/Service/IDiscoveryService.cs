using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gravity.Manager.Data;
using Gravity.Manager.Data.Entities;

namespace Gravity.Manager.Service
{
    public interface IDiscoveryService : IDisposable
    {
        /// <summary>
        /// Gets or creates an AWS account.
        /// </summary>
        /// <returns><see cref="AwsAccount"/></returns>
        Task<AwsAccount> GetOrCreateAwsAccountAsync(string name);

        /// <summary>
        /// Gets the accounts.
        /// </summary>
        Task<List<AwsAccount>> GetAwsAccountsAsync();
            
        /// <summary>
        /// Gets all discovery sessions, including parent <see cref="AwsAccount"/>.
        /// </summary>
        Task<List<DiscoverySession>> GetDiscoverySessionsWithAccountsAsync();
        
        /// <summary>
        /// Gets discovery sessions for specified account.
        /// </summary>
        Task<List<DiscoverySession>> GetDiscoverySessionsWithAccountsAsync(long awsAccountId);

        /// <summary>
        /// Gets the discovery report for the specified discovery session.
        /// </summary>
        Task<DiscoveryReport> GetDiscoveryReportAsync(long discoverySessionId);

        /// <summary>
        /// Populates discovery session information based on provided flat list of dependency findings.
        /// Each finding has a meaning of "Instance X mentions instance Y in file F".
        /// </summary>
        /// <returns><see cref="DiscoverySession"/></returns>
        Task<DiscoverySession> InsertDiscoverySessionAsync(DiscoverySessionInfo discoverySession);
    }
}