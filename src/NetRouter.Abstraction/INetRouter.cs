namespace NetRouter.Abstraction
{
    using NetRouter.Abstraction.Configuration;
    using System;
    using System.Threading.Tasks;

    public interface INetRouter
    {
        void Setup(ISetupConfiguration setupConfiguration);

        Task ProcessRequestAsync(IRequest request);

        Task ProcessRequestAsync(IRequest request, Func<IResponse, Task> responseCallback);
    }
}