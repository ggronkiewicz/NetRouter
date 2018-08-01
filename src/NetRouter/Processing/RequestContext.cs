namespace NetRouter.Processing
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using NetRouter.Abstraction;
    using NetRouter.Message;

    internal class RequestContext : IRequestContext
    {
        public RequestContext(IRequest request)
        {
            Request = request;
        }

        public IRequest Request { get; }

        public HttpClient HttpClient { get; set; }

        public async Task<IResponse> GetResponse()
        {
            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Port = this.Request.UrlHost.Port ?? (this.Request.Protocol.Equals("https", StringComparison.OrdinalIgnoreCase) ? 443 : 80);
            uriBuilder.Host = this.Request.UrlHost.Host;
            uriBuilder.Path = this.Request.UrlPath.Value;
            uriBuilder.Query = this.Request.UrlQuery.Value;
            uriBuilder.Scheme = this.Request.Protocol;

            StreamContent streamContent = null;
            if (this.Request.Body != null && this.Request.Body.Content != null && this.Request.Body.Content.CanRead)
            {
                streamContent = new StreamContent(this.Request.Body.Content);
            }

            HttpRequestMessage message = new HttpRequestMessage(new HttpMethod(this.Request.Method), uriBuilder.Uri);
            message.Content = streamContent;
            if (this.Request.Headers != null)
            {
                foreach (var header in this.Request.Headers)
                {
                    if (!message.Headers.TryAddWithoutValidation(header.Key, header.Value))
                    {
                        message.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }

            var responseMessage = await this.HttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

            var response = new DefaultResponse();
            response.Status = (ushort)responseMessage.StatusCode;

            foreach (var header in responseMessage.Headers)
            {
                response.Headers[header.Key] = header.Value;
            }

            if (responseMessage.Content != null)
            {
                if (responseMessage.Content.Headers != null)
                {
                    foreach (var header in responseMessage.Content.Headers)
                    {
                        response.Headers[header.Key] = header.Value;
                    }
                }

                response.Body = new StreamMessageBody(await responseMessage.Content.ReadAsStreamAsync());
            }

            return response;
        }
    }
}
