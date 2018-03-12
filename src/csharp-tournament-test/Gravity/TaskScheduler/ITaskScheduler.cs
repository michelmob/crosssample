using System.Collections.Generic;

namespace Gravity.TaskScheduler
{
    public interface ITaskScheduler
    {
        List<TaskInfo> GetExecutingJobList();
        List<TaskKey> GetScheduledJobList();
        void RemoveTask(TaskKey taskKey);
        void ScheduleTask(IScheduleTask task);

        void ScheduleTask<TData>(IScheduleTaskWithData<TData> taskWithData)
            where TData : class;

        void Start();
        void Stop();
    }
}