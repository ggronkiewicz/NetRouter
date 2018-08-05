namespace NetRouter.Abstraction.Configuration
{
    using NetRouter.Abstraction.Filters;

    public interface ISetupConfigurationFactory
    {
        ISetupConfigurationFactory AddFilter(IFilter filter);

        ISetupConfigurationFactory AddFilter<TFilter>() where TFilter : IFilter;

        ISetupConfigurationFactory EnableRewind(bool request = true, bool response = true);
    }
}
