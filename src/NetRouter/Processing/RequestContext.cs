namespace NetRouter.Processing
{
    using System.Net.Http;
    using NetRouter.Abstraction;

    internal class RequestContext : IRequestContext
    {
        public RequestContext(IRequest request)
        {
            Request = request;
        }

        public IRequest Request { get; }

        public HttpClient HttpClient { get; set; }
    }
}
