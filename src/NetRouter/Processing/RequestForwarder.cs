namespace NetRouter.Processing
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using NetRouter.Abstraction;
    using NetRouter.Abstraction.Filters;
    using NetRouter.Message;

    internal class RequestForwarder : IFilter
    {
        public async Task<IResponse> Execute(IRequestContext requestContext, FilterAction next)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException(nameof(requestContext));
            }

            if (requestContext.Request == null)
            {
                return null;
            }

            UriBuilder uriBuilder = new UriBuilder();
            uriBuilder.Port = requestContext.Request.UrlHost.Port ?? (requestContext.Request.Protocol.Equals("https", StringComparison.OrdinalIgnoreCase) ? 443 : 80);
            uriBuilder.Host = requestContext.Request.UrlHost.Host;
            uriBuilder.Path = requestContext.Request.UrlPath.Value;
            uriBuilder.Query = requestContext.Request.UrlQuery.Value;
            uriBuilder.Scheme = requestContext.Request.Protocol;

            StreamContent streamContent = null;
            if (requestContext.Request.Body != null && requestContext.Request.Body.Content != null && requestContext.Request.Body.Content.CanRead)
            {
                streamContent = new StreamContent(requestContext.Request.Body.Content);
            }

            HttpRequestMessage message = new HttpRequestMessage(new HttpMethod(requestContext.Request.Method), uriBuilder.Uri);
            message.Content = streamContent;
            if (requestContext.Request.Headers != null)
            {
                foreach (var header in requestContext.Request.Headers)
                {
                    if (!message.Headers.TryAddWithoutValidation(header.Key, header.Value))
                    {
                        message.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }

            var responseMessage = await requestContext.HttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

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
