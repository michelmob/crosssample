using System.Collections.Generic;
using System.Linq;
using Gravity.Manager.Data.EF.Tests;
using Gravity.Manager.Service;
using Gravity.Manager.Tests.Service;
using Gravity.Manager.Web.Controllers;
using Gravity.Manager.Web.Models;
using Gravity.Manager.Web.Models.DiscoveryReport;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Gravity.Manager.Web.Tests.Controllers
{
    public class DiscoveryReportControllerTests
    {
        private IDiscoveryService _service;
        private DiscoveryReportController _controller;

        [SetUp]
        public void TestSetUp()
        {
            _service = DiscoveryServiceTests.GetService();
            _controller = new DiscoveryReportController(_service);

            var acc = _service.GetOrCreateAwsAccountAsync("awsAcc").Result;
            _service.InsertDiscoverySessionAsync(DiscoveryServiceTests.GenerateDiscoverySession(acc)).Wait();
        }

        [TearDown]
        public void TestTearDown()
        {
            _service.Dispose();
            _controller.Dispose();
        }

        [Test]
        public void Controller_Index_ReturnsDiscoveryReports()
        {
            var reports = (ViewResult) _controller.Index().Result;
            var models = (List<DiscoverySessionViewModel>) reports.Model;
            var model = models.Single();

            Assert.AreEqual("awsAcc", model.AwsAccountName);
            Assert.AreEqual(FixedDateTimeProvider.DateTime, model.RunDate);
        }

        [Test]
        public void Controller_Report_ReturnsDiscoveryReportById()
        {
            var reportId = _service.GetDiscoverySessionsWithAccountsAsync().Result.Single().Id;
            var report = (DiscoveryReportViewModel) ((ViewResult) _controller.Report(reportId).Result).Model;
            
            Assert.AreEqual(reportId, report.Id);

            Assert.AreEqual("awsAcc", report.AwsAccountName);
            Assert.AreEqual(FixedDateTimeProvider.DateTime, report.RunDate);
            Assert.AreEqual(new[] {"196.168.1.1", "196.168.1.2", "196.168.1.3"},
                report.AwsInstances.Select(x => x.IpAddress.ToString()).OrderBy(x => x).ToArray());
            
            // Check matrix.
            Assert.AreEqual(3, report.Cells.Length);
            Assert.AreEqual(3, report.Cells[0].Length);
            Assert.AreEqual(3, report.Cells[1].Length);
            
            // Check report contents.
            var line = report.AwsInstances[0].ReportLines.Single();
            
            Assert.AreEqual(DiscoveryServiceTests.RootReportLineName, line.Name);
            Assert.AreEqual(DiscoveryServiceTests.RootReportLineName, line.NameAndValue);
            Assert.IsNull(line.Value);
            Assert.IsNull(line.TableHeaders);
            Assert.IsFalse(line.IsDictionary);
            Assert.IsFalse(line.IsEmpty);
            Assert.IsFalse(line.IsEmptyRoot);

            line = line.Children.Single();
            Assert.IsTrue(line.IsDictionary);

            line = line.Children.Single();
            Assert.AreEqual(DiscoveryServiceTests.LeafReportLineName, line.Name);
            Assert.AreEqual(DiscoveryServiceTests.LeafReportLineValue, line.Value);
            Assert.IsNull(line.Children);
        }

        [Test]
        public void Controller_Report_ReturnsErrorMessageForMissingReport()
        {
            var reportId = (long) ((ViewResult) _controller.Report(-1).Result).Model;
            
            Assert.AreEqual(-1, reportId);
        }
    }
}