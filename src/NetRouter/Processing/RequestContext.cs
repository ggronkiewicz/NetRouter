namespace NetRouter.Processing
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;
    using NetRouter.Abstraction;

    internal class RequestContext : IRequestContext
    {
        private readonly static object locker = new object();
        private readonly ILogger logger;
        private List<IDisposable> disposableList;

        public RequestContext(IRequest request, ILogger logger)
        {
            Request = request;
            this.logger = logger;
        }

        public IRequest Request { get; }

        public HttpClient HttpClient { get; set; }

        public void Dispose()
        {
            lock (locker)
            {
                disposableList?.ForEach(x =>
                {
                    try
                    {
                        x.Dispose();
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError(ex, "Error in dispose object " + x.GetType().FullName);
                    }
                });

                this.disposableList?.Clear();
                this.HttpClient = null;
            }
        }

        public void RegisterForDispose(IDisposable disposable)
        {
            if (disposable != null)
            {
                this.disposableList = disposableList ?? new List<IDisposable>();
                disposableList.Add(disposable);
            }
        }
    }
}
