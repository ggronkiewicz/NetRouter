namespace NetRouter.Abstraction
{
    using System;
    using System.Net.Http;

    public interface IRequestContext : IDisposable
    {
        IRequest Request { get; }

        HttpClient HttpClient { get; set; }

        void RegisterForDispose(IDisposable disposable);
    }
}
