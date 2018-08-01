namespace NetRouter
{
    using Microsoft.Extensions.DependencyInjection;
    using NetRouter.Abstraction;
    using NetRouter.Processing;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNetRouter(this IServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
            {
                throw new System.ArgumentNullException(nameof(serviceCollection));
            }

            serviceCollection.AddSingleton<INetRouter, NetRouterServer>();

            return serviceCollection;
        }
    }
}
