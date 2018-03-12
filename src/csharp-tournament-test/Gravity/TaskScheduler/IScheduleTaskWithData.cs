namespace Gravity.TaskScheduler
{
    public interface IScheduleTaskWithData<TData> : IScheduleTask where TData : class
    {
        TData TaskData { get; set; }
    }
}