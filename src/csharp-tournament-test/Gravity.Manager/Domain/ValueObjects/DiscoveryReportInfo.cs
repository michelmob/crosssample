using System.Collections.Generic;
using System.Net;
using Gravity.Manager.Domain.Aws;

namespace Gravity.Manager.Domain.ValueObjects
{
    public class DiscoveryReportInfo
    {
        public IPAddress AwsInstanceIpAddress { get; set; }
        
        public ICollection<ReportLine> ReportLines { get; set; }
    }
}