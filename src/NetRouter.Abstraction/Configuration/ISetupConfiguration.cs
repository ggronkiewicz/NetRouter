namespace NetRouter.Abstraction.Configuration
{
    using NetRouter.Abstraction.Filters;
    using System.Collections.Generic;

    public interface ISetupConfiguration
    {
        IList<IFilter> Filters { get; }
    }
}
