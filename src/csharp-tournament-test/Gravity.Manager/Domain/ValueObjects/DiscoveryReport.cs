using System;
using System.Collections.Generic;
using System.Linq;
using Gravity.Manager.Domain.Aws;
using Gravity.Manager.Domain.Dependencies;

namespace Gravity.Manager.Domain.ValueObjects
{
    /// <summary>
    /// Represents entire discovery report: dependency matrix and report data.
    /// </summary>
    public sealed class DiscoveryReport
    {
        public DiscoveryReport(IList<DependencyFinding> dependencies)
        {
            if (dependencies == null || dependencies.Count == 0)
            {
                throw new ArgumentException("Dependency matrix can not be emtpy", nameof(dependencies));
            }

            Session = dependencies.Select(x => x.Dependency.SourceAwsInstance.DiscoverySession).First();
            var instances = Session.AwsInstances;
            var count = instances.Count;
            
            DependencyMatrix = Enumerable.Range(0, count).Select(x => new Dependency[count]).ToArray();
            var indexes = instances.Select((inst, idx) => new {inst, idx}).ToDictionary(x => x.inst, x => x.idx);

            foreach (var dependency in dependencies.Select(x => x.Dependency).Distinct())
            {
                var row = indexes[dependency.SourceAwsInstance];
                var col = indexes[dependency.TargetAwsInstance];

                DependencyMatrix[row] = DependencyMatrix[row] ?? new Dependency[count];

                DependencyMatrix[row][col] = dependency;
            }
        }
        
        public DiscoverySession Session { get; }

        public Dependency[][] DependencyMatrix { get; }
    }
}