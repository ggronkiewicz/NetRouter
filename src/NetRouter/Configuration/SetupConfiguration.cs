namespace NetRouter.Configuration
{
    using System.Collections.Generic;
    using NetRouter.Abstraction.Configuration;
    using NetRouter.Abstraction.Filters;

    public class SetupConfiguration : ISetupConfiguration
    {
        public IList<IFilter> Filters { get; }

        public bool EnabledRewindRequest { get; set; }

        public bool EnabledRewindResponse { get; set; }

        public SetupConfiguration()
        {
            this.Filters = new List<IFilter>();
        }
    }
}
