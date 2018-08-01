//  --------------------------------------------------------------------------------------------------------------------
namespace NetRouter.Tests.Filters.Routing
{
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;

    using FluentAssertions;

    using Moq;

    using NetRouter.Configuration.Routing;
    using NetRouter.Filters.Routing;
    using NetRouter.Routing;

    using Xunit;

    public class HttpClientFactoryTests
    {
        [Fact]
        public void CreateDefaultTest()
        {
            var factory = CreateHttpClientFactory();

            var client = factory.Create(new HttpClientConfiguration());

            client.Should().NotBeNull();
        }

        [Fact]
        public void CreateWithSslCertificateTest()
        {
            var factory = CreateHttpClientFactory();

            HttpClientConfiguration configuration = new HttpClientConfiguration
            {
                Certificates = "certName",
                SslProtocol = "Tls11|Tls12"
            };
            var client = factory.Create(configuration);

            configuration.SslProtocolInternal.Should().Be(SslProtocols.Tls11 | SslProtocols.Tls12);
            client.Should().NotBeNull();
            // todo: veryfication of settings
        }

        [Fact]
        public void CacheTest()
        {
            var factory = CreateHttpClientFactory();

            HttpClientConfiguration configuration = new HttpClientConfiguration
            {
                Certificates = "certName",
                SslProtocol = "Tls12"
            };
            var client1 = factory.Create(configuration);
            var client2 = factory.Create(configuration);

            client1.Should().Be(client2);

            configuration.SslProtocol = "Tls11";
            var client3 = factory.Create(configuration);

            client3.Should().NotBe(client1);
        }

        [Fact]
        public void NullTest()
        {
            var factory = CreateHttpClientFactory();

            var client = factory.Create(null);

            client.Should().NotBeNull();
        }

        private static HttpClientFactory CreateHttpClientFactory()
        {
            var providerMock = new Mock<ICertificateProvider>();
            providerMock.Setup(x => x.GetCertyficate(It.IsAny<string>())).Returns(new X509Certificate2());
            var factory = new HttpClientFactory(providerMock.Object);
            return factory;
        }
    }
}