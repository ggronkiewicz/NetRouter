namespace NetRouter.Processing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using global::NetRouter.Abstraction;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;
    using NetRouter.Abstraction.Configuration;
    using NetRouter.Abstraction.Filters;
    using NetRouter.Message;

    public class NetRouterServer : INetRouter
    {
        private FilterAction routerSteps;
        private ISetupConfiguration setupConfiguration;
        private readonly ILogger logger;

        public NetRouterServer(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory?.CreateLogger("NetRouter");
        }

        public async Task ProcessHttpContext(HttpContext context, Func<Task> next)
        {
            var request = new DefaultRequest(context.Request);
            try
            {
                var response = await this.ProcessRequest(request);
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

                if (response.Body != null)
                {
                    await response.Body.Content.CopyToAsync(context.Response.Body);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error");

                context.Response.StatusCode = 500;
                var body = Encoding.UTF8.GetBytes(ex.Message);
                context.Response.Body.Write(body, 0, body.Length);
            }
        }

        public async Task<IResponse> ProcessRequest(IRequest request)
        {
            var context = new RequestContext(request);

            return await routerSteps(context);
        }

        public void Setup(ISetupConfiguration configuration)
        {
            this.setupConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var list = new List<IFilter>(configuration.Filters);
            list.Add(new RequestForwarder());

            this.routerSteps = FiltersPipelineFactory.Create(list, this.logger);
        }
    }
}