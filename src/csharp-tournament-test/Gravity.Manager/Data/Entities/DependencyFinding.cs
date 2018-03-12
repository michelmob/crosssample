using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Gravity.Data;

namespace Gravity.Manager.Data.Entities
{
    /// <summary>
    /// Dependency finding explains why the dependency between AWS instances exists:
    /// Instance X is mentioned in file on instance Y, etc.
    /// </summary>
    public class DependencyFinding : EntityBase
    {
        [Required]
        [ForeignKey(nameof(Dependency))]
        public long DependencyId { get; set; }

        [Required]
        [MaxLength(4096)]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the snippet of text that caused the dependency finding
        /// (from log file, config file, etc; can include surrounding lines for context).
        /// </summary>
        [Required]
        [MaxLength(8192)]
        public string Text { get; set; }
        
        public Dependency Dependency { get; set; }

        [ExcludeFromCodeCoverage]
        protected override string ToPropertyString()
        {
            return $"{base.ToPropertyString()}, {nameof(DependencyId)}: {DependencyId}, {nameof(FileName)}: {FileName}";
        }
    }
}