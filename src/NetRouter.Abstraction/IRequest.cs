namespace NetRouter.Abstraction
{
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;

    public interface IRequest
    {
        string Method { get; set; }

        PathString UrlPath { get; set; }

        QueryString UrlQuery { get; set; }

        HostString UrlHost { get; set; }

        string Protocol { get; set; }

        IDictionary<string, IEnumerable<string>> Headers { get; }

        IMessageBody Body { get; }
    }
}