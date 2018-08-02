namespace NetRouter.Abstraction.Filters
{
    using System.Threading.Tasks;

    public delegate Task<IResponse> FilterAction(IRequestContext requestContext);
}
