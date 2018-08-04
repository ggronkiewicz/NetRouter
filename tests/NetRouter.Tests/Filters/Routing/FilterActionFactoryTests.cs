namespace NetRouter.Tests.Filters.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using NetRouter.Abstraction;
    using NetRouter.Abstraction.Filters;
    using NetRouter.Filters.Common;
    using NetRouter.Filters.Routing.Configuration;
    using NetRouter.Filters.Routing.MappingFilters;
    using Xunit;

    public class FilterActionFactoryTests
    {
        [Fact]
        public void NoFiltersTest()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();

            FilterActionFactory filterActionFactory = new FilterActionFactory(mockServiceProvider.Object, new ConfigurationContainerMock());

            FilterAction lastAction = cxt => Task.FromResult<IResponse>(null);
            var action = filterActionFactory.CreateFilterAction(lastAction, null);

            action.Should().Be(lastAction);
        }

        [Fact]
        public void ExecutedTest()
        {
            var filterMock = new FilterMock();
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(x => x.GetService(It.IsAny<Type>())).Returns(filterMock);

            FilterActionFactory filterActionFactory = new FilterActionFactory(mockServiceProvider.Object, new ConfigurationContainerMock());

            var action = filterActionFactory.CreateFilterAction(cxt => Task.FromResult<IResponse>(null), new[] { "test" });
            action(new Mock<IRequestContext>().Object);

            filterMock.Executed.Should().BeTrue();
        }

        private class ConfigurationContainerMock : IConfigurationContainer
        {
            public void Configure<T>(string sectionName, Action<T> configurationCallback)
            {
                Dictionary<string, MappingFilterConfiguration> dictionary = new Dictionary<string, MappingFilterConfiguration>();
                dictionary.Add("test", new MappingFilterConfiguration
                {
                    Type = typeof(FilterMock).AssemblyQualifiedName
                });

                configurationCallback((T)(object)dictionary);
            }
        }
    }

    public class FilterMock : IFilter
    {
        public bool Executed
        {
            get;
            private set;
        } = false;

        public Task<IResponse> Execute(IRequestContext requestContext, FilterAction next)
        {
            this.Executed = true;
            return next(requestContext);
        }
    }
}
