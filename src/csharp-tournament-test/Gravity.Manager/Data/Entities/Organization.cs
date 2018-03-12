using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gravity.Data;

namespace Gravity.Manager.Data.Entities
{
    public class Organization: EntityBase
    {
        public long? ParentId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [ForeignKey(nameof(ParentId))]
        public Organization Parent { get; set; }
    }
}