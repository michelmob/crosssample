using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Gravity.Data;

namespace Gravity.Manager.Data.Entities
{
    /// <summary>
    /// AWS Instance: tied to a session.
    /// Each session has a set of one or more AWS instances.
    /// </summary>
    public class AwsInstance : EntityBase
    {
        [Required]
        [ForeignKey(nameof(DiscoverySession))]
        public long DiscoverySessionId { get; set; }

        [MaxLength(16)]  // IPv6 is 128 bit
        public byte[] IpAddressBytes { get; set; }

        [NotMapped]
        public IPAddress IpAddress
        {
            get => new IPAddress(IpAddressBytes);
            set => IpAddressBytes = value?.GetAddressBytes();
        }

        public DiscoverySession DiscoverySession { get; set; }
        
        public List<ReportLine> ReportLines { get; set; }

        [ExcludeFromCodeCoverage]
        protected override string ToPropertyString()
        {
            return $"{base.ToPropertyString()}, {nameof(IpAddress)}: {IpAddress}, {nameof(DiscoverySessionId)}: {DiscoverySessionId}";
        }
    }
}