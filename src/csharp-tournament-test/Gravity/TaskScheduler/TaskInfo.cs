namespace Gravity.TaskScheduler
{
    public class TaskInfo
    {
        public TaskKey TaskKey { get; set; }

        public string CronExpression { get; set; }
    }
}