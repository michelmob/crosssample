namespace Gravity.Application
{
    public interface IMappingTask<in TMappingConfiguration> where TMappingConfiguration : class
    {
        int Order { get; set; }

        void ConfigureMapping(TMappingConfiguration configuration);
    }
}