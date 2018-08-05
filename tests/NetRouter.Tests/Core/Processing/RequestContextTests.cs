namespace NetRouter.Tests.Core.Processing
{
    using System;
    using Moq;
    using NetRouter.Abstraction;
    using NetRouter.Processing;
    using Xunit;

    public class RequestContextTests
    {
        [Fact]
        public void DisposeTest()
        {
            var disposableMock = new Mock<IDisposable>();
            using (RequestContext context = new RequestContext(new Mock<IRequest>().Object, null))
            {
                context.RegisterForDispose(disposableMock.Object);
            }

            disposableMock.Verify(x => x.Dispose(), Times.Once());
        }

        [Fact]
        public void DisposeTwiceTest()
        {
            var disposableMock = new Mock<IDisposable>();
            using (RequestContext context = new RequestContext(new Mock<IRequest>().Object, null))
            {
                context.RegisterForDispose(disposableMock.Object);

                context.Dispose();
                disposableMock.Verify(x => x.Dispose(), Times.Once());
            }

            disposableMock.Verify(x => x.Dispose(), Times.Once());
        }
    }
}
