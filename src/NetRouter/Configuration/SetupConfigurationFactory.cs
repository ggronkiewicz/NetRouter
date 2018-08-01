namespace NetRouter.Configuration
{
    using System;

    using NetRouter.Abstraction.Configuration;
    using NetRouter.Abstraction.Filters;

    internal class SetupConfigurationFactory : ISetupConfigurationFactory
    {
        private readonly IServiceProvider serviceProvider;

        private SetupConfiguration configuration;

        public ISetupConfiguration Configuration => this.configuration;

        public SetupConfigurationFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.configuration = new SetupConfiguration();
        }

        public ISetupConfigurationFactory AddFilter(IFilter filter)
        {
            if (filter == null)
            {
                throw new System.ArgumentNullException(nameof(filter));
            }

            this.configuration.Filters.Add(filter);
            return this;
        }

        public ISetupConfigurationFactory AddFilter<TFilter>()
            where TFilter : IFilter
        {
            var filter = this.serviceProvider.GetService(typeof(TFilter)) as IFilter;
            this.configuration.Filters.Add(filter);
            return this;
        }
    }
}
