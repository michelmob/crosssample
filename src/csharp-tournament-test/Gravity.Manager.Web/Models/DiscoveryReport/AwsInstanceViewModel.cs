using System;
using System.Linq;
using System.Net;
using Gravity.Manager.Data.Entities;

namespace Gravity.Manager.Web.Models.DiscoveryReport
{
    public class AwsInstanceViewModel
    {
        public AwsInstanceViewModel(AwsInstance awsInstance)
        {
            awsInstance = awsInstance ?? throw new ArgumentNullException(nameof(awsInstance));

            Id = awsInstance.Id;
            IpAddress = awsInstance.IpAddress;
            ReportLines = awsInstance.ReportLines.Where(x => x.Parent == null)
                .Select(x => new ReportLineViewModel(x))
                .Where(x => !x.IsEmptyRoot)
                .OrderBy(x => x.Name)
                .ToArray();
        }

        public long Id { get; }
        
        public IPAddress IpAddress { get; }
        
        public ReportLineViewModel[] ReportLines { get; }
        
    }
}