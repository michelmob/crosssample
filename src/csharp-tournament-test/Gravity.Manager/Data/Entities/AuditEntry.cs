using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gravity.Data;

namespace Gravity.Manager.Data.Entities
{
    /// <summary>
    /// Audit entry reflects any change made to an entity.
    /// Created: <see cref="NewValue"/> is present, <see cref="OldValue"/> is null.
    /// Updated: <see cref="NewValue"/> is present, <see cref="OldValue"/> is present.
    /// Deleted: <see cref="NewValue"/> is null, <see cref="OldValue"/> is present.
    /// </summary>
    public class AuditEntry : EntityBase
    {
        [Required]
        [MaxLength(100)]
        public string EntityName { get; set; }
        
        [Required]
        [ForeignKey(nameof(User))]
        public long UserId { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [Column("OldValue", TypeName = DataTypeStrings.VarcharMax)]
        public string OldValue { get; set; }
        
        [Column("NewValue", TypeName = DataTypeStrings.VarcharMax)]
        public string NewValue { get; set; }
        
        public User User { get; set; }

        protected override string ToPropertyString()
        {
            return base.ToPropertyString() + ", " +
                   $"{nameof(EntityName)}: {EntityName}, {nameof(UserId)}: {UserId}, {nameof(Date)}: {Date}";
        }
    }
}