namespace NetRouter.Filters.Routing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    using NetRouter.Configuration.Routing;
    using NetRouter.Filters.Common;
    using NetRouter.Filters.Exceptions;

    public class CertificateProvider : ICertificateProvider, IDisposable
    {
        private ConcurrentDictionary<string, Tuple<X509Certificate2, CertificateConfiguration>> cacheDictionary;

        public CertificateProvider(IConfigurationContainer configurationContainer)
        {
            this.cacheDictionary = new ConcurrentDictionary<string, Tuple<X509Certificate2, CertificateConfiguration>>(StringComparer.OrdinalIgnoreCase);
            configurationContainer.Configure<Dictionary<string, CertificateConfiguration>>(Constants.RoutingCertificatesConfigurationSection, this.BindConfigure);
        }

        private void BindConfigure(Dictionary<string, CertificateConfiguration> configuration)
        {
            if (configuration == null || configuration.Any() == false)
            {
                this.cacheDictionary.Clear();
                return;
            }

            foreach (var certificateConfiguration in configuration)
            {
                this.cacheDictionary.AddOrUpdate(
                    certificateConfiguration.Key,
                    name => new Tuple<X509Certificate2, CertificateConfiguration>(this.LoadCertyficate(certificateConfiguration.Value), certificateConfiguration.Value),
                    (name, value) =>
                    {
                        if (value.Item2.GetHashCode() == certificateConfiguration.Value.GetHashCode())
                        {
                            return value;
                        }
                        return new Tuple<X509Certificate2, CertificateConfiguration>(
                            this.LoadCertyficate(certificateConfiguration.Value),
                            certificateConfiguration.Value);
                    });
            }
        }

        private X509Certificate2 LoadCertyficate(CertificateConfiguration configuration)
        {
            var file = configuration.FilePath;
            if (Path.IsPathRooted(file) == false)
            {
                file = Path.Combine(Directory.GetCurrentDirectory(), file);
            }

            if (File.Exists(file) == false)
            {
                throw new FiltersConfigurationException($"Certificate file not found {file}");
            }

            try
            {
                var data = File.ReadAllBytes(file);

                return new X509Certificate2(data, configuration.Password, X509KeyStorageFlags.MachineKeySet);
            }
            catch (Exception e)
            {
                throw new FiltersConfigurationException($"Error from loading certificate {file}", e);
            }
        }

        public X509Certificate2 GetCertyficate(string configurationName)
        {
            if (this.cacheDictionary.TryGetValue(configurationName, out var value))
            {
                return value.Item1;
            }

            throw new FiltersConfigurationException($"Missing configuration for certificate '{configurationName}'");
        }

        public void Dispose()
        {
            this.cacheDictionary?.Values.ToList().ForEach(x => x.Item1.Dispose());
            this.cacheDictionary = null;
        }
    }
}