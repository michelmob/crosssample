using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Gravity.Data;

namespace Gravity.Manager.Data.Entities
{
    /// <summary>
    /// Discovery session: agent is launched on one or more AWS instances and sends discovery data back.
    /// </summary>
    public class DiscoverySession : EntityBase
    {
        [Required]
        [ForeignKey(nameof(AwsAccount))]
        public long AwsAccountId { get; set; }

        [Required]
        public DateTime RunDate { get; set; }
        
        public AwsAccount AwsAccount { get; set; }
        public List<AwsInstance> AwsInstances { get; set; }

        [ExcludeFromCodeCoverage]
        protected override string ToPropertyString()
        {
            return $"{base.ToPropertyString()}, {nameof(AwsAccountId)}: {AwsAccountId}, {nameof(RunDate)}: {RunDate}";
        }
    }
}