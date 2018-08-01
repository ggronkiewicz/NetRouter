namespace NetRouter.Routing
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Net.Http;

    using NetRouter.Configuration.Routing;
    using NetRouter.Filters.Routing;

    internal class HttpClientFactory : IHttpClientFactory
    {
        private readonly ICertificateProvider certificateProvider;

        private readonly ConcurrentDictionary<HttpClientConfiguration, HttpClient> cacheDictionary =
            new ConcurrentDictionary<HttpClientConfiguration, HttpClient>();

        public HttpClientFactory(ICertificateProvider certificateProvider)
        {
            this.certificateProvider = certificateProvider;
        }

        public HttpClient Create(HttpClientConfiguration configuration)
        {
            configuration = configuration ?? new HttpClientConfiguration();
            return cacheDictionary.GetOrAdd(configuration, this.CreateInternal);
        }

        private HttpClient CreateInternal(HttpClientConfiguration configuration)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.AllowAutoRedirect = configuration.AllowAutoRedirect;
            handler.SslProtocols = configuration.SslProtocolInternal;

            if (configuration.ClientCertificateOption.HasValue)
                handler.ClientCertificateOptions = configuration.ClientCertificateOption.Value;

            if (!string.IsNullOrEmpty(configuration.Certificates))
            {
                var certs = configuration.Certificates.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(this.certificateProvider.GetCertyficate)
                    .ToArray();

                handler.ClientCertificates.AddRange(certs);
            }

            return new HttpClient(handler);
        }
    }
}