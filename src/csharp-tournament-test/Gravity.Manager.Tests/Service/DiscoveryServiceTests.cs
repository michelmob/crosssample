using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Gravity.Manager.ApplicationService;
using Gravity.Manager.Data;
using Gravity.Manager.Data.EF;
using Gravity.Manager.Data.EF.Tests;
using Gravity.Manager.Domain.Aws;
using Gravity.Manager.Domain.ValueObjects;
using NUnit.Framework;

namespace Gravity.Manager.Tests.Service
{
    public class DiscoveryServiceTests
    {
        public const string RootReportLineName = "JavaProcesses";
        public const string LeafReportLineName = "Command";
        public const string LeafReportLineValue = "java -Xmx=256m";

        [Test]
        public async Task Service_GetOrCreateAwsAccount_CanBeRetrieved()
        {
            var svc = GetService();
            Assert.IsEmpty(await svc.GetAwsAccountsAsync());

            var createdAcc = await svc.GetOrCreateAwsAccountAsync("acc1");
            var existingAcc = await svc.GetOrCreateAwsAccountAsync("acc1");
            Assert.AreEqual(createdAcc.Id, existingAcc.Id);
            
            var createdAcc2 = await svc.GetOrCreateAwsAccountAsync("acc2");
            Assert.AreNotEqual(createdAcc.Id, createdAcc2.Id);

            var accounts = await svc.GetAwsAccountsAsync();
            Assert.AreEqual(new[] {createdAcc.Id, createdAcc2.Id}, accounts.Select(x => x.Id).OrderBy(x => x));
        }

        [Test]
        public void Service_GetDependencyMatrixForMissingSession_ReturnsNull()
        {
            var service = GetService();

            Assert.IsNotNull(service);
            
            var res = service.GetDiscoveryReportAsync(-1).Result;

            Assert.IsNull(res);
        }

        [Test]
        public void Service_InsertEmptyDiscoverySession_Fails()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
                GetService().InsertDiscoverySessionAsync(new DiscoverySessionInfo
                {
                    AwsAccountId = 1,
                    Dependencies = new DependencyInfo[0],
                    DiscoveryReports = new DiscoveryReportInfo[0]
                }));

            Assert.AreEqual("Discovery session contains no findings.", ex.Message.Substring(0, 39));
        }

        [Test]
        public void Service_InsertEmptyDiscoveryReport_Fails()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(() =>
                GetService().InsertDiscoverySessionAsync(new DiscoverySessionInfo
                {
                    AwsAccountId = 1,
                    Dependencies = GenerateDependencyFindings(),
                    DiscoveryReports = new DiscoveryReportInfo[0]
                }));

            Assert.AreEqual("Discovery reports are missing for some of the instances present in the dependency " +
                            "findings: 196.168.1.1, 196.168.1.2, 196.168.1.3", ex.Message);
        }

        [Test]
        public void Service_InsertDiscoverySession_ValidatesDependencyInfo()
        {
            string AssertThrows(DependencyInfo dep) => Assert.ThrowsAsync<ArgumentException>(
                    () => GetService().InsertDiscoverySessionAsync(new DiscoverySessionInfo
                {
                    AwsAccountId = 1,
                    Dependencies = new []{dep},
                    DiscoveryReports = new DiscoveryReportInfo[0]
                }))
                .Message.Split(Environment.NewLine).First();

            var ex = AssertThrows(new DependencyInfo());
            Assert.AreEqual("DependencyInfo.FileName can not be null or empty.", ex);

            ex = AssertThrows(new DependencyInfo {FileName = "foo"});
            Assert.AreEqual("DependencyInfo.Text can not be null or empty.", ex);

            ex = AssertThrows(new DependencyInfo {FileName = "foo", Text = "text"});
            Assert.AreEqual("DependencyInfo.Source can not be null.", ex);
            
            ex = AssertThrows(new DependencyInfo {FileName = "foo", Text = "text", Source = IPAddress.Loopback});
            Assert.AreEqual("DependencyInfo.Target can not be null.", ex);
        }

        [Test]
        public async Task Service_InsertDiscoverySession_CreatesDependencyMatrix()
        {
            var matrix = await InsertDiscoverySessionAsync();

            var arr = matrix.DependencyMatrix;
            Assert.AreEqual(3, arr.Length);  // 3 unique instances
            
            var inst = matrix.Session.AwsInstances;
            Assert.AreEqual(new[] {"196.168.1.1", "196.168.1.2", "196.168.1.3"},
                inst.Select(x => x.IpAddress.ToString()).OrderBy(x => x).ToArray());
            
            // 1 mentions 2 with 2 findings
            var dep = arr[0][1];
            Assert.AreSame(inst[0], dep.SourceAwsInstance);
            Assert.AreSame(inst[1], dep.TargetAwsInstance);
            CollectionAssert.AreEquivalent(new[] {"web.config", "myconf"},
                dep.DependencyFindings.Select(x => x.FileName));
            
            // does not mention 1 or 3.
            Assert.IsNull(arr[0][0]);
            Assert.IsNull(arr[0][2]);
            
            // 2 mentions 1 with 1 finding. 
            dep = arr[1][0];
            Assert.AreSame(inst[1], dep.SourceAwsInstance);
            Assert.AreSame(inst[0], dep.TargetAwsInstance);
            Assert.AreEqual("settings.xml", dep.DependencyFindings.Single().FileName);
            
            // does not mention 2 or 3.
            Assert.IsNull(arr[1][1]);
            Assert.IsNull(arr[1][2]);
            
            // 3 mentions 1 with 2 findings.
            dep = arr[2][0];
            Assert.AreSame(inst[2], dep.SourceAwsInstance);
            Assert.AreSame(inst[0], dep.TargetAwsInstance);
            CollectionAssert.AreEquivalent(new[] {"nginx.log", "nginx.log2"},
                dep.DependencyFindings.Select(x => x.FileName));
            
            // does not mention 2 or 3.
            Assert.IsNull(arr[2][1]);
            Assert.IsNull(arr[2][2]);
        }

        [Test]
        public static async Task Service_InsertDiscoverySession_CreatesDependencyReport()
        {
            var report = await InsertDiscoverySessionAsync();

            foreach (var awsInstance in report.Session.AwsInstances)
            {
                var lines = awsInstance.ReportLines;
                
                // Order is generated automatically.
                Assert.AreEqual(new[] {0, 1, 2}, lines.Select(x => x.Order).ToArray());
                
                // Hierarchy is preserved.
                Assert.AreEqual(lines, new[] {lines[0]}.Flatten(x => x.Children));
                Assert.AreEqual(new[] {RootReportLineName, null, LeafReportLineName}, lines.Select(x => x.Name).ToArray());
            }
        }

        private static async Task<DiscoveryReport> InsertDiscoverySessionAsync()
        {
            var svc = GetService();
            var acc = await svc.GetOrCreateAwsAccountAsync("foo");
            Assert.IsEmpty(await svc.GetDiscoverySessionsWithAccountsAsync(acc.Id));

            // Insert session.
            var session = GenerateDiscoverySession(acc);
            var sesId = (await svc.InsertDiscoverySessionAsync(session)).Id;

            // Verify session.
            var ses = (await svc.GetDiscoverySessionsWithAccountsAsync(acc.Id)).Single();
            Assert.AreEqual(acc.Id, ses.AwsAccountId);
            Assert.AreEqual(sesId, ses.Id);
            Assert.AreEqual(ses.RunDate, FixedDateTimeProvider.DateTime);

            // Verify dependency matrix.
            var matrix = await svc.GetDiscoveryReportAsync(sesId);
            Assert.AreEqual(sesId, matrix.Session.Id);
            Assert.AreEqual(acc.Id, matrix.Session.AwsAccountId);
            Assert.AreEqual(acc.Name, matrix.Session.AwsAccount.Name);
            return matrix;
        }

        public static DiscoverySessionInfo GenerateDiscoverySession(AwsAccount acc)
        {
            return new DiscoverySessionInfo
            {
                AwsAccountId = acc.Id,
                Dependencies = GenerateDependencyFindings(),
                DiscoveryReports = GenerateDependencyReports().ToArray()
            };
        }

        private static IEnumerable<DiscoveryReportInfo> GenerateDependencyReports()
        {
            return GenerateDependencyFindings()
                .SelectMany(x => new[] {x.Source, x.Target})
                .Distinct()
                .Select(x => new DiscoveryReportInfo
                {
                    AwsInstanceIpAddress = x,
                    ReportLines = new[]
                    {
                        new ReportLine
                        {
                            Name = RootReportLineName,
                            Children = new List<ReportLine>
                            {
                                new ReportLine
                                {
                                    Children = new List<ReportLine>
                                    {
                                        new ReportLine
                                        {
                                            Name = LeafReportLineName,
                                            Value = LeafReportLineValue
                                        }
                                    }
                                }
                            }
                        }
                    }.Flatten(l => l.Children).ToArray()
                });
        }

        private static DependencyInfo[] GenerateDependencyFindings()
        {
            return new[]
            {
                new DependencyInfo
                {
                    FileName = "web.config",
                    Source = IPAddress.Parse("196.168.1.1"),
                    Target = IPAddress.Parse("196.168.1.2"),
                    Text = "endpoint='196.168.1.2'"
                },
                new DependencyInfo
                {
                    FileName = "myconf",
                    Source = IPAddress.Parse("196.168.1.1"),
                    Target = IPAddress.Parse("196.168.1.2"),
                    Text = "server=196.168.1.2"
                },
                new DependencyInfo
                {
                    FileName = "settings.xml",
                    Source = IPAddress.Parse("196.168.1.2"),
                    Target = IPAddress.Parse("196.168.1.1"),
                    Text = "connectTo:196.168.1.1"
                },
                new DependencyInfo
                {
                    FileName = "nginx.log",
                    Source = IPAddress.Parse("196.168.1.3"),
                    Target = IPAddress.Parse("196.168.1.1"),
                    Text = "source:'196.168.1.1'"
                },
                new DependencyInfo
                {
                    FileName = "nginx.log2",
                    Source = IPAddress.Parse("196.168.1.3"),
                    Target = IPAddress.Parse("196.168.1.1"),
                    Text = "source:'196.168.1.1'"
                }
            };
        }

        public static IDiscoveryAppService GetService()
        {
            var dbContext = GravityManagerDbContextTests.GetTestDbContext();

            var ctx = new DiscoveryUnitOfWork(dbContext);
            return new DiscoveryAppService(ctx, new FixedDateTimeProvider());
        }
    }
}