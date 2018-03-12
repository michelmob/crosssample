using System.Net;

namespace Gravity.Manager.Domain.ValueObjects
{
    /// <summary>
    /// Represents dependency information received from the agent.
    /// </summary>
    public class DependencyInfo
    {
        /// <summary>
        /// Source IP address.
        /// </summary>
        public IPAddress Source { get; set; }
        
        /// <summary>
        /// Target IP address.
        /// </summary>
        public IPAddress Target { get; set; }
        
        /// <summary>
        /// File name where dependency has been detected.
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// Text snippet with dependency information.
        /// </summary>
        public string Text { get; set; }
    }
}