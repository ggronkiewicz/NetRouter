namespace NetRouter.Abstraction
{
    using NetRouter.Abstraction.Configuration;
    using System.Threading.Tasks;

    public interface INetRouter
    {
        void Setup(ISetupConfiguration setupConfiguration);

        Task<IResponse> ProcessRequest(IRequest request);
    }
}