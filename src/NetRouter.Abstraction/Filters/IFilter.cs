namespace NetRouter.Abstraction.Filters
{
    using System.Threading.Tasks;

    public interface IFilter
    {
        Task<IResponse> Execute(IRequestContext requestContext, FilterAction next);
    }
}
