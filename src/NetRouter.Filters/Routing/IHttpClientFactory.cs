//  --------------------------------------------------------------------------------------------------------------------
namespace NetRouter.Routing
{
    using System.Net.Http;

    using NetRouter.Configuration.Routing;

    public interface IHttpClientFactory
    {
        HttpClient Create(HttpClientConfiguration configuration);
    }
}