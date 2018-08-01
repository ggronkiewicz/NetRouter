namespace NetRouter.Configuration
{
    using System.Collections.Generic;
    using NetRouter.Abstraction.Configuration;
    using NetRouter.Abstraction.Filters;

    public class SetupConfiguration : ISetupConfiguration
    {
        public IList<IFilter> Filters { get; }

        public SetupConfiguration()
        {
            this.Filters = new List<IFilter>();
        }
    }
}
