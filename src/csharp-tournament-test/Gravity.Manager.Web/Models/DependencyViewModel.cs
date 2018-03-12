using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Gravity.Manager.Data;
using Gravity.Manager.Domain.ValueObjects;

namespace Gravity.Manager.Web.Models
{
    public sealed class DependencyViewModel : IValidatableObject
    {
        /// <summary>
        /// Source IP address.
        /// </summary>
        public string SourceIp { get; set; }
        
        /// <summary>
        /// Target IP address.
        /// </summary>
        public string TargetIp { get; set; }
        
        /// <summary>
        /// File name where dependency has been detected.
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// Text snippet with dependency information.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Validates values and converts this instance to <see cref="DependencyInfo"/>.
        /// </summary>
        /// <returns>Resulting <see cref="DependencyInfo"/> or a validation error.</returns>
        public DependencyInfo ToDependencyInfo()
        {
            return new DependencyInfo
            {
                FileName = FileName,
                Text =  Text,
                Source = IPAddress.Parse(SourceIp),
                Target = IPAddress.Parse(TargetIp),
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(FileName))
            {
                yield return new ValidationResult($"{nameof(FileName)} can not be null or whitespace.");
            }
            
            if (string.IsNullOrWhiteSpace(Text))
            {
                yield return new ValidationResult($"{nameof(Text)} can not be null or whitespace.");
            }
            
            if (!IPAddress.TryParse(SourceIp, out var _))
            {
                yield return new ValidationResult("Failed to parse IP address: " + nameof(SourceIp));
            }
            
            if (!IPAddress.TryParse(TargetIp, out var _))
            {
                yield return new ValidationResult("Failed to parse IP address: " + nameof(TargetIp));
            }
        }
    }
}