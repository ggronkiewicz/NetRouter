namespace NetRouter.Tests.Core.Processing
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using NetRouter.Abstraction;
    using NetRouter.Abstraction.Filters;
    using NetRouter.Processing;
    using Xunit;

    public class FiltersPipelineFactoryTests
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
                FiltersPipelineFactory.Create(null, null);
            }
            catch (ArgumentException)
            {
                return;
            }

            throw new Exception("FiltersPipelineFactory.Create should throw exception for null");
        }

        [Fact]
        public async Task EmptyTest()
        {
            var pipeline = FiltersPipelineFactory.Create(new IFilter[0], null);

            (await pipeline(Context)).Should().BeNull();
        }

        [Fact]
        public void PipelineTest()
        {
            var filterMock1 = new Mock<IFilter>();
            filterMock1.Setup(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()))
                .Returns<IRequestContext, FilterAction>((ctx, next) => next(ctx));
            var filterMock2 = new Mock<IFilter>();
            filterMock2.Setup(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()))
                .Returns<IRequestContext, FilterAction>((ctx, next) => next(ctx));

            var pipeline = FiltersPipelineFactory.Create(new[] { filterMock1.Object, filterMock2.Object }, null);
            pipeline(Context);

            filterMock1.Verify(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()), Times.Once);
            filterMock2.Verify(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()), Times.Once);
        }

        [Fact]
        public void PipelineWithBreakTest()
        {
            var filterMock1 = new Mock<IFilter>();
            filterMock1.Setup(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()))
                .Returns<IRequestContext, FilterAction>((ctx, next) => next(ctx));
            var filterMock2 = new Mock<IFilter>();
            filterMock2.Setup(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()))
                .Returns<IRequestContext, FilterAction>((ctx, next) => { return null; });
            filterMock2.Setup(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>())).ReturnsAsync((IResponse)null);
            var filterMock3 = new Mock<IFilter>();

            var pipeline = FiltersPipelineFactory.Create(new[] { filterMock1.Object, filterMock2.Object }, null);
            pipeline(Context);

            filterMock1.Verify(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()), Times.Once);
            filterMock3.Verify(x => x.Execute(It.IsAny<IRequestContext>(), It.IsAny<FilterAction>()), Times.Never);
        }
    }
}
