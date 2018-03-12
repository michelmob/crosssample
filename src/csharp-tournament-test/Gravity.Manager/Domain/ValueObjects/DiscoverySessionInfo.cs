using System.Collections.Generic;

namespace Gravity.Manager.Domain.ValueObjects
{
    public class DiscoverySessionInfo
    {
        public long AwsAccountId { get; set; }
        
        public ICollection<DependencyInfo> Dependencies { get; set; }
        
        public ICollection<DiscoveryReportInfo> DiscoveryReports { get; set; }
        
    }
}