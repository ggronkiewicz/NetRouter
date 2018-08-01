namespace NetRouter.Filters.Routing
{
    using System.Security.Cryptography.X509Certificates;

    public interface ICertificateProvider
    {
        X509Certificate2 GetCertyficate(string configurationName);
    }
}