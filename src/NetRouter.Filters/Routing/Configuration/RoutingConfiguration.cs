namespace NetRouter.Configuration.Routing
{
    using System.Collections.Generic;

    public class RoutingConfiguration
    {
        public Dictionary<string, Mapping> Mappings { get; set; }

        public Dictionary<string, CertificateConfiguration> Certificates { get; set; }
    }
}