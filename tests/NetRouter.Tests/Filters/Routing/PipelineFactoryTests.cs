using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NetRouter.Abstraction;
using NetRouter.Abstraction.Filters;
using NetRouter.Filters.Routing.MappingFilters;
using Xunit;

namespace NetRouter.Tests.Filters.Routing
{
    public class PipelineFactoryTests
    {
        private IRequestContext Context
        {
            get
            {
                var mock = new Mock<IRequestContext>();
                return mock.Object;
            }
        }

        [Fact]
        public void NullTest()
        {
            try
            {
                PipelineFactory.Create(null, null);
            }
            catch (ArgumentException)
            {
                return;
            }

            throw new Exception("PipelineFactory.Create should throw exception for null");
        }

        [Fact]
        public async Task EmptyTest()
        {
            bool isCalled = false;
            FilterAction filterAction = ctx => { isCalled = true; return Task.FromResult<IResponse>(null); };
            var pipeline = PipelineFactory.Create(new IFilter[0], filterAction);

            (await pipeline(Context)).Should().BeNull();
            isCalled.Should().BeTrue();
        }

        [Fact]
        public async Task PipelineTestAsync()
        {
            FilterAction filterAction = ctx => Task.FromResult<IResponse>(null);

            var filterMock1 = new Mock<IFilter>();
            filterMock1.Setup(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()))
                .Returns<IRequestContext, FilterAction>((ctx, next) => next(ctx));
            var filterMock2 = new Mock<IFilter>();
            filterMock2.Setup(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()))
                .Returns<IRequestContext, FilterAction>((ctx, next) => next(ctx));

            var pipeline = PipelineFactory.Create(new[] { filterMock1.Object, filterMock2.Object }, filterAction);
            await pipeline(Context);

            filterMock1.Verify(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()), Times.Once);
            filterMock2.Verify(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()), Times.Once);
        }

        [Fact]
        public async Task PipelineWithBreakTestAsync()
        {
            FilterAction filterAction = ctx => Task.FromResult<IResponse>(null);

            var filterMock1 = new Mock<IFilter>();
            filterMock1.Setup(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()))
                .Returns<IRequestContext, FilterAction>((ctx, next) => next(ctx));
            var filterMock2 = new Mock<IFilter>();
            filterMock2.Setup(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()))
                .Returns<IRequestContext, FilterAction>((ctx, next) => { return Task.FromResult<IResponse>(null); });
            filterMock2.Setup(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>())).ReturnsAsync((IResponse)null);
            var filterMock3 = new Mock<IFilter>();

            var pipeline = PipelineFactory.Create(new[] { filterMock1.Object, filterMock2.Object }, filterAction);
            await pipeline(Context);

            filterMock1.Verify(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()), Times.Once);
            filterMock3.Verify(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()), Times.Never);
        }
    }
}
