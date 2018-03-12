using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Gravity.Data;

namespace Gravity.Manager.Data.Entities
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
        
        public AwsInstance SourceAwsInstance { get; set; }
        
        public AwsInstance TargetAwsInstance { get; set; }
        
        public List<DependencyFinding> DependencyFindings { get; set; }

        [ExcludeFromCodeCoverage]
        protected override string ToPropertyString()
        {
            return $"{base.ToPropertyString()}, {nameof(SourceAwsInstanceId)}: {SourceAwsInstanceId}, {nameof(TargetAwsInstanceId)}: {TargetAwsInstanceId}";
        }
    }
}