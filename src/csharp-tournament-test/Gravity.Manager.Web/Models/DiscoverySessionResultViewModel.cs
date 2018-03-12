using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using Gravity.Manager.Data;
using Gravity.Manager.Domain.Aws;
using Gravity.Manager.Domain.ValueObjects;
using Newtonsoft.Json.Linq;

namespace Gravity.Manager.Web.Models
{
    /// <summary>
    /// Represents a result of a discovery session:
    /// dependency findings, discovery report.
    /// </summary>
    public class DiscoverySessionResultViewModel : IValidatableObject
    {
        /// <summary>
        /// Findings.
        /// </summary>
        [Required]
        public DependencyViewModel[] DependencyFindings { get; set; }
        
        /// <summary>
        /// Report object (would be JObject with Json serializer).
        /// </summary>
        [Required]
        public DiscoveryResultViewModel[] DiscoveryReports { get; set; }

        public DiscoverySessionInfo ToDiscoverySessionInfo()
        {
            return new DiscoverySessionInfo
            {
                Dependencies = DependencyFindings.Select(x => x.ToDependencyInfo()).ToArray(),
                DiscoveryReports = GetDiscoveryReports().ToArray()
            };
        }

        private IEnumerable<DiscoveryReportInfo> GetDiscoveryReports()
        {
            foreach (var report in DiscoveryReports ?? throw new ArgumentNullException())
            {
                yield return new DiscoveryReportInfo
                {
                    AwsInstanceIpAddress = IPAddress.Parse(report.AwsInstanceIpAddress),
                    ReportLines = GetReportLines(null, report.ReportData ?? throw new ArgumentNullException()).ToArray()
                };
            }
        }

        private static IEnumerable<ReportLine> GetReportLines(ReportLine parent, JToken obj)
        {
            var isTable = obj is JArray;

            if (parent != null)
            {
                parent.IsTable = isTable;
            }

            foreach (var token in obj)
            {
                var line = parent;
                
                if (token is JProperty prop)
                {
                    line = new ReportLine
                    {
                        Name = prop.Name,
                        // Do not store values for complex properties (JObject or JArray).
                        Value = prop.Value is JValue ? prop.Value.ToString() : null,
                        Parent = parent,
                    };
                
                    yield return line;
                }
                else if (isTable)
                {
                    // Array element.
                    line = new ReportLine
                    {
                        Parent = parent,
                        // Store string value for leaf properties (no children).
                        Value = token.HasValues ? null : token.ToString()
                    };

                    yield return line;
                }

                foreach (var child in GetReportLines(line, token))
                {
                    yield return child;
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DependencyFindings == null || DependencyFindings.Length == 0)
            {
                yield return new ValidationResult("Request contains no dependency findings",
                    new[] {nameof(DependencyFindings)});
            }
            else
            {
                foreach (var validationResult in DependencyFindings.SelectMany(x => x.Validate(validationContext)))
                {
                    yield return validationResult;
                }
            }

            if (DiscoveryReports == null || DiscoveryReports.Length == 0)
            {
                yield return new ValidationResult("Request contains no discovery reports",
                    new[] {nameof(DiscoveryReports)});
            }
            else
            {
                foreach (var validationResult in DiscoveryReports.SelectMany(x => x.Validate(validationContext)))
                {
                    yield return validationResult;
                }
            }

            if (DependencyFindings != null && DiscoveryReports != null)
            {
                var dependencyIps = new HashSet<string>(
                    DependencyFindings.SelectMany(x => new[] {x.SourceIp, x.TargetIp}));

                var discoveryIps = new HashSet<string>(DiscoveryReports.Select(x => x.AwsInstanceIpAddress));

                if (discoveryIps.Count < DiscoveryReports.Length)
                {
                    yield return new ValidationResult($"{nameof(DiscoveryReports)} contain duplicate AWS instances.");
                }

                if (dependencyIps.Count != discoveryIps.Count)
                {
                    yield return new ValidationResult(
                        $"{nameof(DependencyFindings)} mention {dependencyIps.Count} AWS instances, " +
                        $"but {nameof(DiscoveryReports)} has {DiscoveryReports.Length} items.");
                }

                foreach (var discoveryIp in discoveryIps)
                {
                    if (!dependencyIps.Contains(discoveryIp))
                    {
                        yield return new ValidationResult(
                            $"{nameof(DependencyFindings)} do not contain AWS instance with IP {discoveryIp}.");
                    }
                }
            }
        }
    }
}