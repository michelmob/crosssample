using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Gravity.Data;

namespace Gravity.Manager.Domain.Organizations
{
    public class User : EntityBase
    {
        public long? OrganizationId { get; set; }
        
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [MaxLength(100)]
        public string EMail { get; set; }

        [Required]
        public RoleType Role { get; set; }
        
        [ForeignKey(nameof(OrganizationId))]
        public Organization Organization { get; set; }
        
        [ExcludeFromCodeCoverage]
        protected override string ToPropertyString()
        {
            return $"{base.ToPropertyString()}, {nameof(Name)}: {Name}, {nameof(EMail)}: {EMail}, " +
                   $"{nameof(UserName)}: {UserName}, {nameof(Role)}: {Role}";
        }
    }
}