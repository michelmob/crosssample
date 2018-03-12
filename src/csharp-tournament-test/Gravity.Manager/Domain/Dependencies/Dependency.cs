using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Gravity.Data;
using Gravity.Manager.Domain.Aws;

namespace Gravity.Manager.Domain.Dependencies
{
    /// <summary>
    /// Dependency between AWS instances.
    /// </summary>
    public class Dependency : EntityBase
    {
        [Required]
        [ForeignKey(nameof(SourceAwsInstance))]
        public long SourceAwsInstanceId { get; set; }

        [Required]
        [ForeignKey(nameof(TargetAwsInstance))]
        public long TargetAwsInstanceId { get; set; }
        
        public virtual AwsInstance SourceAwsInstance { get; set; }
        
        public virtual AwsInstance TargetAwsInstance { get; set; }
        
        public virtual List<DependencyFinding> DependencyFindings { get; set; }

        [ExcludeFromCodeCoverage]
        protected override string ToPropertyString()
        {
            return $"{base.ToPropertyString()}, {nameof(SourceAwsInstanceId)}: {SourceAwsInstanceId}, {nameof(TargetAwsInstanceId)}: {TargetAwsInstanceId}";
        }
    }
}