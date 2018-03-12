using System.Linq;
using System.Threading.Tasks;
using Gravity.Manager.ApplicationService;
using Gravity.Manager.Domain.Aws;
using Gravity.Manager.Service;
using Gravity.Manager.Tests.Service;
using Gravity.Manager.Web.Controllers.API;
using Gravity.Manager.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Gravity.Manager.Web.Tests.Controllers
{
    public class DiscoveryControllerTests
    {
        private IDiscoveryAppService _service;
        private TestLogger _logger;
        private DiscoveryController _controller;

        [SetUp]
        public void TestSetUp()
        {
            _service = DiscoveryServiceTests.GetService();
            _logger = new TestLogger();
            _controller = new DiscoveryController(_service, _logger);
        }

        [TearDown]
        public void TestTearDown()
        {
            _service.Dispose();
            _controller.Dispose();
        }

        [Test]
        public async Task DiscoveryController_Post_CreatesDiscoveryReport()
        {
            var discoveryResult = new DiscoverySessionResultViewModel
            {
                DependencyFindings = new[]
                {
                    new DependencyViewModel
                    {
                        FileName = "log.txt",
                        SourceIp = "1.2.3.4",
                        TargetIp = "5.6.7.8",
                        Text = "foo"
                    },
                    new DependencyViewModel
                    {
                        FileName = "config",
                        SourceIp = "4.3.2.1",
                        TargetIp = "1.2.3.4",
                        Text = "bar"
                    }
                },
                DiscoveryReports = new[]
                {
                    new DiscoveryResultViewModel
                    {
                        AwsInstanceIpAddress = "1.2.3.4",
                        ReportData = JObject.Parse("{\"LocalIpv4\": \"1.2.3.4\"}")
                    },
                    new DiscoveryResultViewModel
                    {
                        AwsInstanceIpAddress = "5.6.7.8",
                        ReportData = JObject.Parse("{\"LocalIpv4\": \"5.6.7.8\"}")
                    },
                    new DiscoveryResultViewModel
                    {
                        AwsInstanceIpAddress = "4.3.2.1",
                        ReportData = JObject.Parse("{\"LocalIpv4\": \"4.3.2.1\"}")
                    }
                }
            };

            var findings = discoveryResult.DependencyFindings;
            
            var status = (StatusCodeResult) await _controller.Post("awsAcc1", discoveryResult);
            
            // Check status.
            Assert.AreEqual(StatusCodes.Status200OK, status.StatusCode);
            
            // Check matrix.
            var acc = (await _service.GetAwsAccountsAsync()).Single();
            Assert.AreEqual("awsAcc1", acc.Name);
            
            var ses = (await _service.GetDiscoverySessionsWithAccountsAsync(acc.Id)).Single();
            var report = await _service.GetDiscoveryReportAsync(ses.Id);
            var instances = report.Session.AwsInstances.ToArray();

            Assert.AreEqual(new[] {"1.2.3.4", "5.6.7.8", "4.3.2.1"}, instances.Select(x => x.IpAddress.ToString()));
            
            // 1 -> 2
            var cell = report.DependencyMatrix[0][1];
            Assert.AreEqual(instances[0], cell.SourceAwsInstance); 
            Assert.AreEqual(instances[1], cell.TargetAwsInstance);

            var finding = cell.DependencyFindings.Single();
            Assert.AreEqual(findings[0].FileName, finding.FileName);
            Assert.AreEqual(findings[0].Text, finding.Text);
            
            // 3 -> 1
            var cell2 = report.DependencyMatrix[2][0];
            Assert.AreEqual(instances[2], cell2.SourceAwsInstance); 
            Assert.AreEqual(instances[0], cell2.TargetAwsInstance);
            
            var finding2 = cell2.DependencyFindings.Single();
            Assert.AreEqual(findings[1].FileName, finding2.FileName);
            Assert.AreEqual(findings[1].Text, finding2.Text);
            
            // Check report.
            foreach (var inst in instances)
            {
                var line = inst.ReportLines.Single();
                
                Assert.AreEqual(inst, line.AwsInstance);
                Assert.AreEqual("LocalIpv4", line.Name);
                Assert.AreEqual(inst.IpAddress.ToString(), line.Value);
                Assert.IsNull(line.Parent);
                Assert.IsFalse(line.IsObject);
                Assert.AreEqual(ReportLineStatus.None, line.Status);
            }
        }
        
        [Test]
        public void DiscoveryController_PostInvalidData_ReturnsBadRequest()
        {
            var controller = new DiscoveryController(_service, _logger) {ControllerContext = new ControllerContext()};

            controller.ModelState.AddModelError("foo", "bar");

            var res = (ObjectResult)controller.Post(null, null).Result;
            
            Assert.AreEqual(StatusCodes.Status400BadRequest, res.StatusCode);
            Assert.AreEqual("bar", ((ValidationErrorsViewModel)res.Value).Errors.Single().Message);
        }

        [Test]
        public void DiscoverySessionResultViewModel_Validates()
        {
            // Missing properties.
            Assert.AreEqual("FileName can not be null or whitespace.", PostBadRequest());
            Assert.AreEqual("Text can not be null or whitespace.", PostBadRequest("myFile"));

            // Missing IP addresses.
            Assert.AreEqual("Failed to parse IP address: SourceIp", PostBadRequest("myFile", "txt"));
            Assert.AreEqual("Failed to parse IP address: TargetIp", PostBadRequest("myFile", "txt", "0.1.2.3"));
            
            // Unparseable IP addresses.
            Assert.AreEqual("Failed to parse IP address: SourceIp", PostBadRequest("myFile", "txt", "srcIp", "targetIp"));
            Assert.AreEqual("Failed to parse IP address: TargetIp", PostBadRequest("myFile", "txt", "0.1.2.3", "targetIp"));
            
            // Empty report data.
            Assert.AreEqual("ReportData can not be null or empty.", PostBadRequest("myFile", "txt", "0.1.2.3", "1.2.3.4"));
            Assert.AreEqual("ReportData can not be null or empty.", PostBadRequest("myFile", "txt", "0.1.2.3", "1.2.3.4", "{}"));
            
            // Unparseable IP addresses in reports.
            Assert.AreEqual("Failed to parse IP address: AwsInstanceIpAddress", 
                PostBadRequest("myFile", "txt", "0.1.2.3", "1.2.3.4", "{\"foo\": \"bar\"}", new[]{"0.1.2."}));
            
            // IP addresses mismatch.
            Assert.AreEqual("DependencyFindings mention 2 AWS instances, but DiscoveryReports has 1 items.", 
                PostBadRequest("myFile", "txt", "0.1.2.3", "1.2.3.4", "{\"foo\": \"bar\"}", new[]{"0.1.2.3"}));
            Assert.AreEqual("DependencyFindings do not contain AWS instance with IP 0.1.2.4.", 
                PostBadRequest("myFile", "txt", "0.1.2.3", "1.2.3.4", "{\"foo\": \"bar\"}", new[]{"0.1.2.3", "0.1.2.4"}));
            Assert.AreEqual("DiscoveryReports contain duplicate AWS instances.", 
                PostBadRequest("myFile", "txt", "0.1.2.3", "1.2.3.4", "{\"foo\": \"bar\"}", new[]{"0.1.2.3", "0.1.2.3"}));
        }

        private static string PostBadRequest(string fileName = null, string text = null,
            string sourceIp = null, string targetIp = null, string reportData = null, string[] ips = null)
        {
            var model = new DiscoverySessionResultViewModel
            {
                DependencyFindings = new[]
                {
                    new DependencyViewModel
                    {
                        FileName = fileName,
                        Text = text,
                        SourceIp = sourceIp,
                        TargetIp = targetIp
                    }
                },
                DiscoveryReports = (ips ?? new[]{sourceIp, targetIp}).Select(x =>
                    new DiscoveryResultViewModel
                    {
                        AwsInstanceIpAddress = x,
                        ReportData = reportData == null ? null : JObject.Parse(reportData)
                    }).ToArray()
            };

            return ModelValidator.Validate(model).FirstOrDefault()?.ToString();
        }
    }
}