namespace NetRouter.Processing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using NetRouter.Abstraction;
    using NetRouter.Abstraction.Filters;
    using NetRouter.Exceptions;

    public static class FiltersPipelineFactory
    {
        private static readonly FilterAction EmptyAction = context => Task.FromResult<IResponse>(null);

        public static FilterAction Create(IEnumerable<IFilter> filters, ILogger logger)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            var filtersArray = filters.ToArray();

            var last = EmptyAction;
            for (int i = filtersArray.Length - 1; i >= 0; i--)
            {
                var item = filtersArray[i];
                var nextStep = last;
                last = async (context) =>
                {
                    try
                    {
                        return await item.Execute(context, nextStep);
                    }
                    catch (Exception ex)
                    {
                        throw new NetRouterFilterException(ex, item);
                    }
                };
            }

            return last;
        }
    }
}
