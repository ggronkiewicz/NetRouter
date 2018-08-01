namespace NetRouter.Filters
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    using NetRouter.Filters.Common;
    using NetRouter.Filters.Routing;
    using NetRouter.Routing;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetRouterFilters(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            serviceCollection.AddSingleton<IConfigurationContainer>(new ConfigurationContainer(configuration));
            serviceCollection.AddSingleton<IHttpClientFactory, HttpClientFactory>();
            serviceCollection.AddSingleton<ICertificateProvider, CertificateProvider>();

            // filters
            serviceCollection.AddSingleton<RouterFilter>();

            return serviceCollection;
        }
    }
}
