namespace NetRouter.Filters.Common
{
    using System;

    public interface IConfigurationContainer
    {
        void Configure<T>(string sectionName, Action<T> configurationCallback);
    }
}
