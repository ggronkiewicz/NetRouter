namespace NetRouter.Configuration.Routing
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Http;

    public class Mapping
    {
        private Random random;

        public PathString Path { get; set; }

        public PathString PathReplace { get; set; }

        public MappingStrategy Strategy { get; set; }

        public HttpClientConfiguration HttpClient { get; set; }

        public MappingProtocol Protocol { get; set; } = MappingProtocol.Http;

        public string[] TargetHosts { get; set; }

        public string[] Filters { get; set; }

        public HostString GetRandomHost()
        {
            if (this.TargetHosts == null || this.TargetHosts.Length == 0)
            {
                throw new MissingMemberException($"Missing TargetHosts for mapping path {this.Path}");
            }

            if (this.TargetHosts.Length == 1)
            {
                return new HostString(this.TargetHosts.First());
            }

            this.random = this.random ?? new Random();
            return new HostString(this.TargetHosts[this.random.Next(0, this.TargetHosts.Length)]);
        }
    }
}