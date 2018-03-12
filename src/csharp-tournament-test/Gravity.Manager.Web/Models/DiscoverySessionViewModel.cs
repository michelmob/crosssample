using System;
using Gravity.Manager.Domain.Aws;

namespace Gravity.Manager.Web.Models
{
    public class DiscoverySessionViewModel
    {
        public DiscoverySessionViewModel(DiscoverySession discoverySession)
        {
            if (discoverySession == null)
            {
                throw new ArgumentNullException(nameof(discoverySession));
            }
            
            Id = discoverySession.Id;
            RunDate = discoverySession.RunDate;
            AwsAccountName = discoverySession.AwsAccount.Name;
        }

        public long Id { get; }
        public DateTime RunDate { get; }
        public string AwsAccountName { get; }
    }
}