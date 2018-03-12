using System.Collections.Generic;
using System.Net;
using Gravity.Manager.Data.Entities;

namespace Gravity.Manager.Data
{
    public class DiscoveryReportInfo
    {
        public IPAddress AwsInstanceIpAddress { get; set; }
        
        public ICollection<ReportLine> ReportLines { get; set; }
    }
}