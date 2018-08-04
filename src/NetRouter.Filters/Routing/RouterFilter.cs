namespace NetRouter.Filters.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using NetRouter.Abstraction;
    using NetRouter.Abstraction.Filters;
    using NetRouter.Configuration.Routing;
    using NetRouter.Filters.Common;
    using NetRouter.Filters.Routing.MappingFilters;
    using NetRouter.Routing;

    public class RouterFilter : IFilter
    {
        private static readonly HashSet<string> hedersForRemove = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Connection",
            "Keep-Alive",
            "Public",
            "Proxy-Authenticate",
            "Transfer-Encoding",
            "Upgrade"
        };

        private RoutingConfiguration routingConfiguration;

        private readonly IHttpClientFactory httpClientFactory;

        private readonly IFilterActionFactory filterActionFactory;

        public RouterFilter(
            IConfigurationContainer configurationContainer,
            IHttpClientFactory httpClientFactory,
            IFilterActionFactory filterActionFactory)
        {
            this.httpClientFactory = httpClientFactory;
            this.filterActionFactory = filterActionFactory;
            configurationContainer.Configure<RoutingConfiguration>(string.Empty, cfg => { this.routingConfiguration = cfg; });
        }

        public async Task<IResponse> Execute(IRequestContext requestContext, FilterAction next)
        {
            var cfg = this.routingConfiguration;
            if (cfg.Mappings == null || cfg.Mappings.Count == 0)
            {
                return null;
            }

            foreach (var cfgMapping in cfg.Mappings)
            {
                if (requestContext.Request.UrlPath.StartsWithSegments(cfgMapping.Value.Path, out var remaining))
                {
                    requestContext.Request.UrlHost = cfgMapping.Value.GetRandomHost();
                    switch (cfgMapping.Value.Strategy)
                    {
                        case MappingStrategy.None:
                            break;
                        case MappingStrategy.StripPath:
                            requestContext.Request.UrlPath = remaining;
                            break;
                        case MappingStrategy.Replace:
                            requestContext.Request.UrlPath = cfgMapping.Value.PathReplace + remaining;
                            break;
                    }

                    requestContext.HttpClient = this.httpClientFactory.Create(cfgMapping.Value.HttpClient);
                    requestContext.Request.Protocol = cfgMapping.Value.Protocol.ToString().ToLower();

                    var host = ClearHeaders(requestContext.Request.Headers, new[] { requestContext.Request.UrlHost.Value });

                    if (cfgMapping.Value.ProcessingAction == null)
                    {
                        cfgMapping.Value.ProcessingAction = this.filterActionFactory.CreateFilterAction(next, cfgMapping.Value.Filters);
                    }

                    IResponse response = await cfgMapping.Value.ProcessingAction(requestContext);
                    ClearHeaders(response?.Headers, host);

                    return response;
                }
            }

            // routing Not Foud
            return null;
        }

        private static IEnumerable<string> ClearHeaders(IDictionary<string, IEnumerable<string>> headers, IEnumerable<string> hostName)
        {
            IEnumerable<string> host = null;
            if (headers != null)
            {
                foreach (var header in hedersForRemove)
                {
                    headers.Remove(header);
                }

                headers.TryGetValue("Host", out host);
                if (hostName != null && hostName.Any())
                {
                    headers["Host"] = hostName;
                }
            }

            return host;
        }
    }
}
