namespace NetRouter
{
    using Microsoft.AspNetCore.Builder;
    using NetRouter.Abstraction;
    using NetRouter.Abstraction.Configuration;
    using NetRouter.Configuration;
    using NetRouter.Exceptions;
    using NetRouter.Processing;
    using System;

    public static class ApplicationBuilderExtensions
    {
        public static void UseNetRouter(this IApplicationBuilder applicationBuilder, Action<ISetupConfigurationFactory> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var configurationFactory = new SetupConfigurationFactory(applicationBuilder.ApplicationServices);
            configure(configurationFactory);

            var router = applicationBuilder.ApplicationServices.GetService(typeof(INetRouter)) as NetRouterServer;
            if (router == null)
            {
                throw new NetRouterConfigurationException("Missing INetRouter implementation, call AddNetRouter first.");
            }

            router.Setup(configurationFactory.Configuration);
            applicationBuilder.Use(router.ProcessHttpContext);
        }
    }
}
