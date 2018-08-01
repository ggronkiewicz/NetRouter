namespace NetRouter.Tests.Filters.Routing
{
    using FluentAssertions;
    using NetRouter.Configuration.Routing;
    using System;
    using System.Collections.Generic;
    using System.Security.Authentication;
    using System.Text;
    using Xunit;

    public class HttpClientConfigurationTests
    {
        [Fact]
        public void GetHashCodeTest()
        {
            var configuration = new HttpClientConfiguration();
            var value = configuration.GetHashCode();

            var configuration2 = new HttpClientConfiguration();
            configuration2.GetHashCode().Should().Be(value);

            configuration2 = new HttpClientConfiguration();
            configuration2.AllowAutoRedirect = !configuration.AllowAutoRedirect;
            configuration2.GetHashCode().Should().NotBe(value);

            configuration2 = new HttpClientConfiguration();
            configuration2.SslProtocol = "Tls12";
            configuration2.GetHashCode().Should().NotBe(value);

            configuration2 = new HttpClientConfiguration();
            configuration2.Certificates = "test";
            configuration2.GetHashCode().Should().NotBe(value);

            configuration2 = new HttpClientConfiguration();
            configuration2.ClientCertificateOption = System.Net.Http.ClientCertificateOption.Automatic;
            configuration2.GetHashCode().Should().NotBe(value);
        }


        [Theory]
        [InlineData(null, SslProtocols.None)]
        [InlineData("", SslProtocols.None)]
        [InlineData("none", SslProtocols.None)]
        [InlineData("TLS12", SslProtocols.Tls12)]
        [InlineData("TLS12|tls11", SslProtocols.Tls11 | SslProtocols.Tls12)]
        public void SslProtocolInternalTest(string sslProtocol, SslProtocols expected)
        {
            var configuration = new HttpClientConfiguration();
            configuration.SslProtocol = sslProtocol;

            configuration.SslProtocolInternal.Should().Be(expected);
        }
    }
}
