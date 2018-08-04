namespace NetRouter.Configuration.Routing
{
    using System.Collections.Generic;
    using NetRouter.Filters.Routing.Configuration;

    public class RoutingConfiguration
    {
        public Dictionary<string, Mapping> Mappings { get; set; }

        public Dictionary<string, MappingFilterConfiguration> Filters { get; set; }

        public Dictionary<string, CertificateConfiguration> Certificates { get; set; }
    }
}