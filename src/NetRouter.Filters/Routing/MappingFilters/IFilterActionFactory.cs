namespace NetRouter.Filters.Routing.MappingFilters
{
    using System.Collections.Generic;
    using NetRouter.Abstraction.Filters;

    public interface IFilterActionFactory
    {
        FilterAction CreateFilterAction(FilterAction lastAction, IEnumerable<string> filters);
    }
}