namespace NetRouter.Kestrel.Filters
{
    using System;
    using System.Threading.Tasks;
    using NetRouter.Abstraction;
    using NetRouter.Abstraction.Filters;

    public class ConsoleFilter : IFilter
    {
        public Task<IResponse> Execute(IRequestContext requestContext, FilterAction next)
        {
            Console.WriteLine("Executed at " + DateTime.Now);

            return next(requestContext);
        }
    }
}
