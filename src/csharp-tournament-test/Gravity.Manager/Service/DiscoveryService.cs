using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Gravity.Manager.Data;
using Gravity.Manager.Data.Entities;
using Gravity.Service;

namespace Gravity.Manager.Service
{
    public class DiscoveryService : IDiscoveryService
    {
        private readonly IDiscoveryUnitOfWork _context;
        private readonly IDateTimeProvider _dateTimeProvider;

        public DiscoveryService(IDiscoveryUnitOfWork context, IDateTimeProvider dateTimeProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public Task<AwsAccount> GetOrCreateAwsAccountAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }
            
            return _context.GetOrCreateAwsAccountAsync(name);
        }

        public Task<List<AwsAccount>> GetAwsAccountsAsync()
        {
            return _context.AwsAccounts.GetAllAsync();
        }

        public Task<List<DiscoverySession>> GetDiscoverySessionsWithAccountsAsync()
        {
            return _context.GetDiscoverySessionsWithAccountsAsync();
        }

        public Task<List<DiscoverySession>> GetDiscoverySessionsWithAccountsAsync(long awsAccountId)
        {
            return _context.DiscoverySessions.FindAllAsync(s => s.AwsAccountId == awsAccountId);
        }

        public async Task<DiscoveryReport> GetDiscoveryReportAsync(long discoverySessionId)
        {
            return new DiscoveryReport(await _context.DependencyFindings.GetSessionFindingsAsync(discoverySessionId));
        }

        public async Task<DiscoverySession> InsertDiscoverySessionAsync(DiscoverySessionInfo session)
        {
            session = session ?? throw new ArgumentNullException(nameof(session));
            
            var ds = new DiscoverySession
            {
                AwsAccountId = session.AwsAccountId,
                RunDate = _dateTimeProvider.Now()
            };
            
            var instances = InsertDependencies(ds, session.Dependencies);
            InsertDiscoveryReports(instances, session.DiscoveryReports);

            await _context.CommitAsync();

            return ds;
        }

        private Dictionary<IPAddress, AwsInstance> InsertDependencies(DiscoverySession ds, IEnumerable<DependencyInfo> dependencies)
        {
            ds = ds ?? throw new Exception(nameof(ds));
            dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            
            var instances = new Dictionary<IPAddress, AwsInstance>();
            var deps = new Dictionary<(IPAddress src, IPAddress dst), Dependency>();

            foreach (var depInfo in dependencies)
            {
                ValidateDependency(depInfo, nameof(dependencies));

                var source = instances.GetOrAdd(depInfo.Source, () => new AwsInstance
                {
                    IpAddress = depInfo.Source,
                    DiscoverySession = ds
                });

                var target = instances.GetOrAdd(depInfo.Target, () => new AwsInstance
                {
                    IpAddress = depInfo.Target,
                    DiscoverySession = ds
                });

                var dep = deps.GetOrAdd((depInfo.Source, depInfo.Target), () => new Dependency
                {
                    SourceAwsInstance = source,
                    TargetAwsInstance = target
                });

                var finding = new DependencyFinding
                {
                    FileName = depInfo.FileName,
                    Text = depInfo.Text,
                    Dependency = dep
                };

                _context.DependencyFindings.Insert(finding);
            }

            if (instances.Count == 0)
            {
                throw new ArgumentException("Discovery session contains no findings.", nameof(dependencies));
            }

            return instances;
        }

        private void InsertDiscoveryReports(IDictionary<IPAddress, AwsInstance> instances,
            IEnumerable<DiscoveryReportInfo> reports)
        {
            instances = instances ?? throw new Exception(nameof(instances));
            reports = reports ?? throw new ArgumentNullException(nameof(reports));
            
            var reportInstances = new HashSet<IPAddress>();

            foreach (var report in reports)
            {
                if (!instances.TryGetValue(report.AwsInstanceIpAddress, out var instance))
                {
                    throw new ArgumentException("DiscoveryReportInfo.AwsInstanceIpAddress does not match " +
                                                "any of the AwsInstances from current discovery session.");
                        
                }
                reportInstances.Add(report.AwsInstanceIpAddress);
                
                uint count = 0;
                foreach (var reportLine in report.ReportLines)
                {
                    if (reportLine == null)
                    {
                        throw new ArgumentNullException(nameof(reportLine));
                    }
                    
                    reportLine.Order = count++;
                    reportLine.AwsInstance = instance;

                    _context.ReportLines.Insert(reportLine);
                }

                if (count == 0)
                {
                    throw new ArgumentException("Report can not be empty.", nameof(reports));
                }
            }
            
            if (reportInstances.Count < instances.Count)
            {
                var missingIps = instances.Keys.Except(reportInstances).Select(x => x.ToString());
                throw new ArgumentException("Discovery reports are missing for some of the instances present " +
                                            "in the dependency findings: " + string.Join(", ", missingIps));
            }
        }

        private static void ValidateDependency(DependencyInfo dep, string paramName)
        {
            if (dep == null)
            {
                throw new ArgumentException($"{nameof(DependencyInfo)} can not be null.", paramName);
            }

            if (string.IsNullOrEmpty(dep.FileName))
            {
                throw new ArgumentException($"{nameof(DependencyInfo)}.{nameof(DependencyInfo.FileName)} can not be null or empty.", paramName);
            }

            if (string.IsNullOrEmpty(dep.Text))
            {
                throw new ArgumentException($"{nameof(DependencyInfo)}.{nameof(DependencyInfo.Text)} can not be null or empty.", paramName);
            }

            if (dep.Source == null)
            {
                throw new ArgumentException($"{nameof(DependencyInfo)}.{nameof(DependencyInfo.Source)} can not be null.", paramName);
            }

            if (dep.Target == null)
            {
                throw new ArgumentException($"{nameof(DependencyInfo)}.{nameof(DependencyInfo.Target)} can not be null.", paramName);
            }
        }

        public void Dispose()
        {

        }
    }
}