namespace NetRouter.Abstraction.Filters
{
    using System;
    using System.Threading.Tasks;

    public interface IFilter
    {
        Task<IResponse> Execute(IRequestContext requestContext, Func<IRequestContext, Task<IResponse>> next);
    }
}
