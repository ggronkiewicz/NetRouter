namespace NetRouter.Message
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Http;
    using global::NetRouter.Abstraction;
    using System.IO;

    public class DefaultRequest : IRequest
    {
        public DefaultRequest(HttpRequest contextRequest)
        {
            this.Method = contextRequest.Method;
            this.UrlPath = contextRequest.Path;
            this.UrlQuery = contextRequest.QueryString;
            this.UrlHost = contextRequest.Host;
            this.Protocol = contextRequest.Protocol;
            this.Headers = contextRequest.Headers.ToDictionary(x => x.Key, y => y.Value as IEnumerable<string>);
            this.Body = this.Method.Equals("get", System.StringComparison.OrdinalIgnoreCase) ? null : contextRequest.Body;
        }

        public string Method { get; set; }

        public IDictionary<string, IEnumerable<string>> Headers { get; }

        public Stream Body { get; set; }

        public PathString UrlPath { get; set; }
        public QueryString UrlQuery { get; set; }
        public HostString UrlHost { get; set; }
        public string Protocol { get; set; }
        public bool Ssl { get; set; }
    }
}