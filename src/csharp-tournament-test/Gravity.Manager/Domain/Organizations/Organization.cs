using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gravity.Data;

namespace Gravity.Manager.Domain.Organizations
{
    public class Organization: EntityBase
    {
        public long? ParentId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [ForeignKey(nameof(ParentId))]
        public virtual Organization Parent { get; set; }
    }
}