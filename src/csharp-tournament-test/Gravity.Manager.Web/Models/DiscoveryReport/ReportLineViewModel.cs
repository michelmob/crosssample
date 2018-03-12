using System;
using System.Linq;
using System.Text;
using Gravity.Manager.Data.Entities;

namespace Gravity.Manager.Web.Models.DiscoveryReport
{
    public class ReportLineViewModel
    {
        public ReportLineViewModel(ReportLine reportLine)
        {
            reportLine = reportLine ?? throw new ArgumentNullException(nameof(reportLine));
            
            if (IsPlugin(reportLine))
            {
                Name = reportLine.Name.Replace("Plugins", "").Replace("Plugin", "") + ": ";
                reportLine = reportLine.Children.Single();
            }

            Id = reportLine.Id;
            Name += reportLine.Name;
            Value = reportLine.Value;
            
            if (reportLine.Children == null || reportLine.Children.Count <= 0)
            {
                return;
            }

            Children = reportLine.Children
                .OrderBy(x => x.Order)
                .Select(x => new ReportLineViewModel(x)).ToArray();

            if (reportLine.IsTable)
            {
                TableHeaders = Children[0].Children?.Select(x => x.Name).ToArray() ?? new[] {"Name"};
            }
            else
            {
                IsDictionary = Children.All(x => x.Name != null && x.Value != null && x.Children == null);
            }
        }

        public ReportLineViewModel(string[] tableHeaders)
        {
            TableHeaders = tableHeaders;
        }

        public long Id { get; }

        public string Name { get; }
        
        public string Value { get; }

        public string[] TableHeaders { get; }

        public ReportLineViewModel[] Children { get; }
        
        public bool IsDictionary { get; }

        public bool IsEmpty => string.IsNullOrEmpty(Name ?? Value) &&
                               (Children == null || Children.All(x => x.IsEmpty));
        
        public bool IsEmptyRoot => Children == null || Children.All(x => x.IsEmpty);

        public string NameAndValue => !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Value)
            ? Name + " = " + Value
            : Name ?? Value;

        public string GetValueAndChildren()
        {
            if (Children == null)
            {
                return Value;
            }

            var sb = new StringBuilder();

            sb.Append(Value);

            var first = true;
            foreach (var child in Children)
            {
                if (!first)
                {
                    sb.Append(", ");
                }

                sb.Append(child.GetValueAndChildren());

                first = false;
            }

            return sb.ToString();
        }


        private static bool IsPlugin(ReportLine reportLine)
        {
            return reportLine.Name != null && reportLine.Name.StartsWith("Plugin") &&
                   reportLine.Children != null && reportLine.Children.Count == 1;
        }

        public override string ToString()
        {
            return $"{nameof(ReportLineViewModel)} [{nameof(Name)}: {Name}]";
        }
    }
}