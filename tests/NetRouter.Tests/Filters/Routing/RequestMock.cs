namespace NetRouter.Tests.Filters.Routing
{
    using Microsoft.AspNetCore.Http;
    using NetRouter.Abstraction;
    using System.Collections.Generic;
    using System.IO;

    class RequestMock : IRequest
    {
        public string Method { get; set; }
        public PathString UrlPath { get; set; }
        public QueryString UrlQuery { get; set; }
        public HostString UrlHost { get; set; }
        public string Protocol { get; set; }

        public IDictionary<string, IEnumerable<string>> Headers { get; set; } = new Dictionary<string, IEnumerable<string>>();

        public Stream Body { get; set; }

        public bool Ssl { get; set; }
    }
}
