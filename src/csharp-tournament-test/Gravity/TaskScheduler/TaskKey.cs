namespace Gravity.TaskScheduler
{
    public class TaskKey
    {
        public TaskKey()
        {
        }

        public TaskKey(string name)
        {
            Name = name;
        }

        public TaskKey(string name, string group)
        {
            Name = name;
            Group = group;
        }

        public string Name { get; set; }

        public string Group { get; set; }
    }
}