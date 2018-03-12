namespace Gravity.Manager.Domain.Aws
{
    /// <summary>
    /// Represents the investigation status of the report line.
    /// </summary>
    public enum ReportLineStatus
    {
        /// <summary>
        /// None (non investigated): default status.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Included (approved).
        /// </summary>
        Included = 1,
        
        /// <summary>
        /// Excluded (declined).
        /// </summary>
        Excluded = 2,
        
        /// <summary>
        /// Not sure: additional investigation required.
        /// </summary>
        Unsure = 3
    }
}