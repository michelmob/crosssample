namespace Gravity.Application
{
    public interface IStartupTask
    {
        int Order { get; set; }

        void Execute();
    }
}