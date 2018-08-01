namespace NetRouter.Filters.Common
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Primitives;
    using System;

    internal class ConfigurationContainer : IConfigurationContainer
    {
        private readonly IConfiguration configuration;

        public ConfigurationContainer(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Configure<T>(string sectionName, Action<T> configurationCallback)
        {
            var section = string.IsNullOrEmpty(sectionName) ? configuration : configuration.GetSection(sectionName);
            var configurationItem = section.Get<T>();
            configurationCallback?.Invoke(configurationItem);

            ChangeToken.OnChange(this.configuration.GetReloadToken, action => {
                var obj = configuration.GetSection(sectionName).Get<T>();
                action?.Invoke(obj);
            }, configurationCallback);
        }
    }
}
