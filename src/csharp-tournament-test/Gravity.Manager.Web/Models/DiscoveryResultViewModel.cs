using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Gravity.Manager.Web.Models
{
    public class DiscoveryResultViewModel : IValidatableObject
    {
        public string AwsInstanceIpAddress { get; set; }
        
        /// <summary>
        /// Report object (would be JObject with Json serializer).
        /// </summary>
        public JObject ReportData { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IPAddress.TryParse(AwsInstanceIpAddress, out var _))
            {
                yield return new ValidationResult("Failed to parse IP address: " + nameof(AwsInstanceIpAddress));
            }

            if (ReportData == null || !ReportData.HasValues)
            {
                yield return new ValidationResult($"{nameof(ReportData)} can not be null or empty.");
            }
        }
    }
}