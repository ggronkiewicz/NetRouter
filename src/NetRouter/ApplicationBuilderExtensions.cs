namespace NetRouter
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;
    using NetRouter.Abstraction;
    using NetRouter.Abstraction.Configuration;
    using NetRouter.Configuration;
    using NetRouter.Exceptions;
    using NetRouter.Message;
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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

            var loggerFactory = applicationBuilder.ApplicationServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            var logger = loggerFactory.CreateLogger("NetRouter");

            var router = applicationBuilder.ApplicationServices.GetService(typeof(INetRouter)) as INetRouter;
            if (router == null)
            {
                throw new NetRouterConfigurationException("Missing INetRouter implementation, call ServiceCollection.AddNetRouter first.");
            }

            router.Setup(configurationFactory.Configuration);
            applicationBuilder.Use(async (context, next) => await HttpRequestExecute(context, router, logger, next));
        }

        internal static async Task HttpRequestExecute(HttpContext context, INetRouter router, ILogger logger, Func<Task> next)
        {
            var request = new DefaultRequest(context.Request);
            try
            {
                await router.ProcessRequestAsync(request, async response =>
                {
                    if (response == null)
                    {
                        await next();
                        return;
                    }

                    context.Response.StatusCode = response.Status;
                    foreach (var header in response.Headers)
                    {
                        context.Response.Headers.Add(header.Key, new StringValues(header.Value.ToArray()));
                    }

                    if (response.Body != null && response.Body.CanRead)
                    {
                        await response.Body.CopyToAsync(context.Response.Body);
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");

                context.Response.StatusCode = 500;
                var body = Encoding.UTF8.GetBytes(ex.GetType().Name + ": " + ex.Message);
                context.Response.Body.Write(body, 0, body.Length);
            }
        }
    }
}
