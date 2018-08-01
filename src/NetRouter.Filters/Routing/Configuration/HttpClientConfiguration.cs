namespace NetRouter.Configuration.Routing
{
    using System.Linq;
    using System.Globalization;
    using System.Security.Authentication;
    using System;
    using System.Net.Http;

    public class HttpClientConfiguration
    {
        private string sslProtocol;

        public string SslProtocol
        {
            get => sslProtocol;
            set
            {
                sslProtocol = value;
                if (string.IsNullOrEmpty(value))
                {
                    this.SslProtocolInternal = SslProtocols.None;
                }
                else
                {
                    this.SslProtocolInternal = value.Split(new[] { ',', '|' })
                        .Where(x => string.IsNullOrEmpty(x) == false)
                        .Select(x => (SslProtocols)Enum.Parse(typeof(SslProtocols), x, true))
                        .Aggregate((x, y) => x | y);
                }
            }
        }

        public string Certificates { get; set; }

        public bool AllowAutoRedirect { get; set; } = false;

        public ClientCertificateOption? ClientCertificateOption { get; internal set; }

        internal SslProtocols SslProtocolInternal { get; private set; } = SslProtocols.None;

        public override int GetHashCode()
        {
            return string.Concat(
                this.SslProtocol,
                this.Certificates,
                this.AllowAutoRedirect.ToString(CultureInfo.InvariantCulture),
                this.ClientCertificateOption ?? System.Net.Http.ClientCertificateOption.Manual ).GetHashCode();
        }
    }
}