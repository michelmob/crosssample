using System;

namespace Gravity.TaskScheduler
{
    public interface IScheduleTaskBase
    {
        string CronExpression { get; set; }
        bool IsActive { get; set; }
        byte Priority { get; set; }
        TaskKey TaskKey { get; set; }
        DateTime? ValidAfter { get; set; }
        DateTime? ValidBefore { get; set; }
    }
}