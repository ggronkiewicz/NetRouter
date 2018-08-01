namespace NetRouter.Filters
{
    using System;

    using NetRouter.Abstraction.Configuration;
    using NetRouter.Filters.Routing;

    public static class SetupConfigurationFactoryExtensions
    {
        public static ISetupConfigurationFactory AddRouter(this ISetupConfigurationFactory configurationFactory)
        {
            if (configurationFactory == null) throw new ArgumentNullException(nameof(configurationFactory));

            configurationFactory.AddFilter<RouterFilter>();

            return configurationFactory;
        }
    }
}