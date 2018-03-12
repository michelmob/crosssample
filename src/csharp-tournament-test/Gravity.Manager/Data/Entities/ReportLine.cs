using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Gravity.Data;

namespace Gravity.Manager.Data.Entities
{
    /// <summary>
    /// Report line: a part of discovery report for the gived AWS instance.
    /// </summary>
    public class ReportLine : EntityBase
    {
        [ForeignKey(nameof(AwsInstance))]
        public long AwsInstanceId { get; set; }
        
        public long? ParentId { get; set; }

        public uint Order { get; set; }
        
        public string Name { get; set; }
        
        public string Value { get; set; }
        
        public ReportLineStatus Status { get; set; }
       
        /// <summary>
        /// Indicates that underlying values have tabular structure.
        /// </summary>
        public bool IsTable { get; set; }
        
        /// <summary>
        /// Indicates that this is an object with nested properties.
        /// </summary>
        public bool IsObject => Value == null && !IsTable;

        [ForeignKey(nameof(ParentId))]
        public ReportLine Parent { get; set; }
        
        [ForeignKey(nameof(ParentId))]
        public List<ReportLine> Children { get; set; }
        
        public AwsInstance AwsInstance { get; set; }

        protected override string ToPropertyString()
        {
            return $"{base.ToPropertyString()}, {nameof(Name)}: {Name}, {nameof(Value)}: {Value}, {nameof(IsTable)}: {IsTable}";
        }
    }
}