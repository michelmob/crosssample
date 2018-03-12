using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gravity.Data
{
    public abstract class EntityBase : IEntity<long>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public override string ToString()
        {
            return $"{GetType().Name} [{ToPropertyString()}]";
        }

        protected virtual string ToPropertyString()
        {
            return $"{nameof(Id)}: {Id}";
        }
    }
}