namespace NetRouter.Abstraction
{
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IRequestContext
    {
        IRequest Request { get; }

        HttpClient HttpClient { get; set; }

        Task<IResponse> GetResponse();
    }
}
