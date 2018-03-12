using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Gravity.Data;

namespace Gravity.Manager.Data.Entities
{
    /// <summary>
    /// AWS Account.
    /// </summary>
    public class AwsAccount : EntityBase
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        
        [ExcludeFromCodeCoverage]
        protected override string ToPropertyString()
        {
            return $"{base.ToPropertyString()}, {nameof(Name)}: {Name}";
        }
    }
}