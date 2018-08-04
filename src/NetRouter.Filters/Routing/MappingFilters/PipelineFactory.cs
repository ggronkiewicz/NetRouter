namespace NetRouter.Filters.Routing.MappingFilters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetRouter.Abstraction.Filters;

    internal static class PipelineFactory
    {
        public static FilterAction Create(IEnumerable<IFilter> filters, FilterAction lastAction)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            if (lastAction == null)
            {
                throw new ArgumentNullException(nameof(lastAction));
            }

            var filtersArray = filters.ToArray();

            var last = lastAction;
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
                        throw new InvalidOperationException("Filter error: " + ex.Message, ex);
                    }
                };
            }

            return last;
        }
    }
}
