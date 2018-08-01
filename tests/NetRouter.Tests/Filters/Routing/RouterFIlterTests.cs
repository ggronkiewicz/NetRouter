namespace NetRouter.Tests.Filters.Routing
{
    using FluentAssertions;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using NetRouter.Abstraction;
    using NetRouter.Abstraction.Filters;
    using NetRouter.Configuration.Routing;
    using NetRouter.Filters.Common;
    using NetRouter.Filters.Routing;
    using NetRouter.Processing;
    using NetRouter.Routing;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    public class RouterFilterTests
    {
        private readonly PathString Path = "/test";
        private readonly PathString ReplacedPath = "/replaced";
        private readonly HostString TargetHost = new HostString("0.0.0.1:80");
        private readonly Func<IRequestContext, Task<IResponse>> emptyNext = context => Task.FromResult<IResponse>(null);

        [Fact]
        public async Task NotMatchPathTest()
        {
            var filter = this.CreateRoutingFilter(MappingStrategy.None);
            IRequestContext context = this.CreateRequestContext("http://localhost/nottest");

            var response = await filter.Execute(context, emptyNext);

            response.Should().BeNull();
        }

        [Fact]
        public async Task MatchPathTest()
        {
            var filter = this.CreateRoutingFilter(MappingStrategy.None);
            IRequestContext context = this.CreateRequestContext("http://localhost/test");

            Func<IRequestContext, Task<IResponse>> next = contextCheck =>
            {
                contextCheck.HttpClient.Should().NotBeNull();
                contextCheck.Request.UrlPath.Should().Be(Path);
                contextCheck.Request.UrlHost.Should().Be(TargetHost);
                return Task.FromResult(new Mock<IResponse>().Object);
            };

            var response = await filter.Execute(context, next);
            response.Should().NotBeNull();
        }

        [Fact]
        public async Task MatchPathAndStripTest()
        {
            PathString newPatch = "/abc";
            var filter = this.CreateRoutingFilter(MappingStrategy.StripPath);
            IRequestContext context = this.CreateRequestContext("http://localhost/test" + newPatch);

            Func<IRequestContext, Task<IResponse>> next = contextCheck =>
            {
                contextCheck.HttpClient.Should().NotBeNull();
                contextCheck.Request.UrlPath.Should().Be(newPatch);
                contextCheck.Request.UrlHost.Should().Be(TargetHost);
                return Task.FromResult(new Mock<IResponse>().Object);
            };

            var response = await filter.Execute(context, next);
            response.Should().NotBeNull();
        }

        [Fact]
        public async Task MatchPathAndReplaceTest()
        {
            var filter = this.CreateRoutingFilter(MappingStrategy.Replace);
            IRequestContext context = this.CreateRequestContext("http://localhost/test");

            Func<IRequestContext, Task<IResponse>> next = contextCheck =>
            {
                contextCheck.HttpClient.Should().NotBeNull();
                contextCheck.Request.UrlPath.Should().Be(ReplacedPath);
                contextCheck.Request.UrlHost.Should().Be(TargetHost);
                return Task.FromResult(new Mock<IResponse>().Object);
            };

            var response = await filter.Execute(context, next);
            response.Should().NotBeNull();
        }

        [Fact]
        public async Task HeadersCleaningTest()
        {
            string okHeader = "Content-Type";
            var headers = new Dictionary<string, IEnumerable<string>>()
            {
                { "Connection" , new[] { "test" }},
                { "Keep-Alive", new[] { "test" }},
                { "Public", new[] { "test" }},
                { "Proxy-Authenticate", new[] { "test" }},
                { "Transfer-Encoding", new[] { "test" }},
                { "Upgrade", new[] { "test" }},
                { okHeader, new[] { "test" }}
            };

            var filter = this.CreateRoutingFilter(MappingStrategy.StripPath);
            IRequestContext context = this.CreateRequestContext("http://localhost/test", headers: headers);

            Func<IRequestContext, Task<IResponse>> next = contextCheck =>
            {
                contextCheck.Request.Headers.Should().ContainKey(okHeader);
                contextCheck.Request.Headers.Should().HaveCount(1);

                return Task.FromResult<IResponse>(null);
            };

            var response = await filter.Execute(context, emptyNext);
        }

        private IFilter CreateRoutingFilter(MappingStrategy strategy = MappingStrategy.None)
        {
            var configuration = new RoutingConfiguration
            {
                Mappings = new Dictionary<string, Mapping>()
            };
            configuration.Mappings.Add("test", new Mapping
            {
                Path = Path,
                PathReplace = ReplacedPath,
                Strategy = strategy,
                TargetHosts = new[] { TargetHost.Value }
            });

            var configurationMock = new Mock<IConfigurationContainer>();
            configurationMock.Setup(x => x.Configure(It.IsAny<string>(), It.IsAny<Action<RoutingConfiguration>>()))
                .Callback<string, Action<RoutingConfiguration>>((config, action) => { action.Invoke(configuration); });

            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(x => x.Create(It.IsAny<HttpClientConfiguration>()))
                .Returns(new HttpClient());

            return new RouterFilter(configurationMock.Object, httpClientFactoryMock.Object);
        }

        private IRequestContext CreateRequestContext(string url, string method = "GET", Dictionary<string, IEnumerable<string>> headers = null)
        {
            var uri = new Uri(url);

            var requestMock = new RequestMock();
            requestMock.UrlHost = new HostString(uri.Host);
            requestMock.UrlPath = new PathString(uri.AbsolutePath);
            requestMock.UrlQuery = new QueryString(uri.Query);
            requestMock.Method = method;
            requestMock.Headers = headers;

            return new RequestContext(requestMock);
        }
    }
}
