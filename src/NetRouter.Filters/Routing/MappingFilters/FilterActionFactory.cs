namespace NetRouter.Filters.Routing.MappingFilters
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using NetRouter.Abstraction.Filters;
    using NetRouter.Filters.Common;
    using NetRouter.Filters.Routing.Configuration;

    internal class FilterActionFactory : IFilterActionFactory
    {
        private ConcurrentDictionary<string, Tuple<IFilter, MappingFilterConfiguration>> filtersDictionary;

        private readonly IServiceProvider serviceProvider;

        public FilterActionFactory(IServiceProvider serviceProvider, IConfigurationContainer configurationContainer)
        {
            this.serviceProvider = serviceProvider;

            filtersDictionary = new ConcurrentDictionary<string, Tuple<IFilter, MappingFilterConfiguration>>(StringComparer.OrdinalIgnoreCase);
            configurationContainer.Configure<Dictionary<string, MappingFilterConfiguration>>("Filters", this.BindConfiguration);
        }

        private void BindConfiguration(Dictionary<string, MappingFilterConfiguration> configuration)
        {
            if (configuration == null || configuration.Any() == false)
            {
                this.filtersDictionary.Clear();
                return;
            }

            foreach (var item in configuration)
            {
                if (!this.filtersDictionary.TryGetValue(item.Key, out var element) || element.Item2.GetHashCode() != item.Value.GetHashCode())
                {
                    var filterType = Type.GetType(item.Value.Type, true);
                    IFilter filter;
                    try
                    {
                        filter = serviceProvider.GetService(filterType) as IFilter ?? Activator.CreateInstance(filterType) as IFilter;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Cannot create instance of filter '{item.Key}' of '{item.Value?.Type}' ", ex);
                    }

                    if (filter == null)
                    {
                        throw new InvalidOperationException($"Cannot create instance of filter '{item.Key}' of '{item.Value?.Type}' ");
                    }

                    filtersDictionary.TryAdd(item.Key, new Tuple<IFilter, MappingFilterConfiguration>(filter, item.Value));
                }
            }
        }

        public FilterAction CreateFilterAction(FilterAction lastAction, IEnumerable<string> filters)
        {
            if (lastAction == null)
            {
                throw new ArgumentNullException(nameof(lastAction));
            }

            if (filters == null || filters.Any() == false)
            {
                return lastAction;
            }

            var filtersInstances = filters.Reverse().Select(x =>
            {
                if (!this.filtersDictionary.TryGetValue(x, out var filertData))
                {
                    throw new InvalidOperationException($"Cannot find filter for name {x}");
                }

                return filertData.Item1;
            });

            return PipelineFactory.Create(filtersInstances, lastAction);
        }
    }
}
