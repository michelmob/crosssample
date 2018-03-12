using System;
using System.Linq;
using Gravity.Manager.Data.Entities;

namespace Gravity.Manager.Web.Models.DiscoveryReport
{
    public class DiscoveryReportViewModel
    {
        public DiscoveryReportViewModel(Data.DiscoveryReport report)
        {
            Id = (report ?? throw new ArgumentNullException(nameof(report))).Session.Id;
            
            AwsAccountName = report.Session.AwsAccount.Name;
            RunDate = report.Session.RunDate;
            Cells = report.DependencyMatrix;

            AwsInstances = report.Session.AwsInstances
                .Select(x => new AwsInstanceViewModel(x))
                .ToArray();
        }

        public long Id { get; }
        public AwsInstanceViewModel[] AwsInstances { get; }
        public Dependency[][] Cells { get; }
        public string AwsAccountName { get; }
        public DateTime RunDate { get; }
    }
}